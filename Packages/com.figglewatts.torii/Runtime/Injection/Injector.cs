using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Torii.Util;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Torii.Injection
{
    /// <summary>
    /// Responsible for dependency injection.
    ///
    /// Heavily inspired by: http://www.what-could-possibly-go-wrong.com/dependency-injection-for-unity-part-1
    /// </summary>
    public class Injector
    {
        internal interface IInjectionSite
        {
            void Resolve(object value);

            string Name { get; }

            Type SiteType { get; }

            From FromSource { get; }

            Search SearchMethod { get; }

            string HintPath { get; }

            MonoBehaviour Script { get; }

            bool Required { get; }
        }

        internal class PropertyInjectionSite : IInjectionSite
        {
            private readonly PropertyInfo _info;

            public PropertyInjectionSite(PropertyInfo info, MonoBehaviour script)
            {
                _info = info;
                var attr = AttributeUtil.GetAttribute<InjectAttribute>(info);
                FromSource = attr.From;
                SearchMethod = attr.Search;
                HintPath = attr.HintPath;
                Script = script;
                Required = attr.Required;
            }

            public void Resolve(object value) { _info.SetValue(Script, value); }

            public string Name => _info.Name;

            public Type SiteType => _info.PropertyType;

            public From FromSource { get; }

            public Search SearchMethod { get; }

            public string HintPath { get; }

            public MonoBehaviour Script { get; }

            public bool Required { get; }
        }

        internal class FieldInjectionSite : IInjectionSite
        {
            private readonly FieldInfo _info;

            public FieldInjectionSite(FieldInfo info, MonoBehaviour script)
            {
                _info = info;
                var attr = AttributeUtil.GetAttribute<InjectAttribute>(info);
                FromSource = attr.From;
                SearchMethod = attr.Search;
                HintPath = attr.HintPath;
                Script = script;
                Required = attr.Required;
            }

            public void Resolve(object value) { _info.SetValue(Script, value); }

            public string Name => _info.Name;

            public Type SiteType => _info.FieldType;

            public From FromSource { get; }

            public Search SearchMethod { get; }

            public string HintPath { get; }

            public MonoBehaviour Script { get; }

            public bool Required { get; }
        }

        private string injectionSiteDebugString(IInjectionSite site)
        {
            return $"object '{site.Script.gameObject.name}' at site {site.Script.name}.{site.Name} ({site.SiteType})";
        }

        private bool isInjectionSite(MemberInfo info) { return AttributeUtil.HasAttribute<InjectAttribute>(info); }

        private bool isAutoInjectable(MonoBehaviour script)
        {
            return AttributeUtil.HasAttribute<AutoInjectAttribute>(script.GetType());
        }

        private IEnumerable<IInjectionSite> getInjectionSites(MonoBehaviour script)
        {
            var type = script.GetType();
            var props = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                            .Where(isInjectionSite)
                            .Select(prop => new PropertyInjectionSite(prop, script))
                            .Cast<IInjectionSite>();
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                             .Where(isInjectionSite)
                             .Select(field => new FieldInjectionSite(field, script))
                             .Cast<IInjectionSite>();
            return props.Concat(fields);
        }

        private IEnumerable<Component> findAllInjectionSources(Type siteType, GameObject sourceObject)
        {
            foreach (var component in sourceObject.GetComponents<Component>())
            {
                if (siteType.IsInstanceOfType(component))
                {
                    yield return component;
                }
            }
        }

        private Component findInjectionSource(IInjectionSite site, GameObject sourceObject)
        {
            var foundSites = findAllInjectionSources(site.SiteType, sourceObject).ToList();
            if (foundSites.Any())
            {
                if (foundSites.Count > 1)
                {
                    Debug.LogWarning($"Found more than one injection source on object " +
                                     $"'{sourceObject}' for {injectionSiteDebugString(site)} - potential " +
                                     $"ambiguity issues. Picking the first one.");
                }

                return foundSites.First();
            }

            return null;
        }

        private List<GameObject> getSceneRootObjects()
        {
            var activeScene = SceneManager.GetActiveScene();
            var rootObjects = new List<GameObject>(activeScene.rootCount);
            activeScene.GetRootGameObjects(rootObjects);
            return rootObjects;
        }

        private GameObject getObjectFromHint(string hintText)
        {
            if (string.IsNullOrEmpty(hintText)) return null;

            var hierarchy = hintText.Split(new[] {'.'}, StringSplitOptions.RemoveEmptyEntries);
            var rootObjects = getSceneRootObjects();

            var queue = new Queue<GameObject>();
            foreach (var obj in rootObjects)
            {
                queue.Enqueue(obj);
            }

            // perform a DFS on the tree of objects, following the hierarchy given by the hint
            int currentHierarchyElt = 0;
            while (queue.Count > 0)
            {
                var obj = queue.Dequeue();
                if (obj.name.Equals(hierarchy[currentHierarchyElt]))
                {
                    if (currentHierarchyElt + 1 >= hierarchy.Length)
                    {
                        // we're at the end of the hierarchy
                        return obj;
                    }

                    queue.Clear();
                    currentHierarchyElt++;
                    foreach (Transform child in obj.transform)
                    {
                        // we need to check the children
                        queue.Enqueue(child.gameObject);
                    }
                }
            }

            return null;
        }

        private IEnumerable<GameObject> depthFirstTraversal(List<GameObject> rootObjects)
        {
            Stack<GameObject> stack = new Stack<GameObject>();

            // push the root nodes in reverse order to maintain pre-order
            for (int i = rootObjects.Count - 1; i >= 0; i--)
            {
                stack.Push(rootObjects[i]);
            }

            // just a bog-standard iterative pre-order traversal
            while (stack.Count > 0)
            {
                GameObject current = stack.Pop();
                yield return current;

                foreach (Transform child in current.transform)
                {
                    stack.Push(child.gameObject);
                }
            }
        }

        private IEnumerable<GameObject> breadthFirstTraversal(List<GameObject> rootObjects)
        {
            Queue<GameObject> queue = new Queue<GameObject>();
            foreach (var node in rootObjects)
            {
                queue.Enqueue(node);
            }

            while (queue.Count > 0)
            {
                GameObject current = queue.Dequeue();
                yield return current;

                foreach (Transform child in current.transform)
                {
                    queue.Enqueue(child.gameObject);
                }
            }
        }

        private bool resolveSiteDepthFirst(IInjectionSite site, List<GameObject> rootNodes)
        {
            foreach (var gameObject in depthFirstTraversal(rootNodes))
            {
                var source = findInjectionSource(site, gameObject);
                if (source != null)
                {
                    site.Resolve(source);
                    return true;
                }
            }

            if (site.Required)
            {
                Debug.LogWarning("Unable to find dependency source from anywhere (used depth-first strategy)");
            }

            return false;
        }

        private bool resolveSiteBreadthFirst(IInjectionSite site, List<GameObject> rootNodes)
        {
            foreach (var gameObject in breadthFirstTraversal(rootNodes))
            {
                var source = findInjectionSource(site, gameObject);
                if (source != null)
                {
                    site.Resolve(source);
                    return true;
                }
            }

            if (site.Required)
            {
                Debug.LogWarning("Unable to find dependency source from anywhere (used breadth-first strategy)");
            }

            return false;
        }

        private bool resolveSiteFromObject(IInjectionSite site, GameObject gameObject)
        {
            switch (site.SearchMethod)
            {
                case Search.Breadth:
                    return resolveSiteBreadthFirst(site, new List<GameObject> {gameObject});
                case Search.Depth:
                    return resolveSiteDepthFirst(site, new List<GameObject> {gameObject});
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private bool resolveSiteFromRootObjects(IInjectionSite site)
        {
            var rootObjects = getSceneRootObjects();
            switch (site.SearchMethod)
            {
                case Search.Breadth:
                    return resolveSiteBreadthFirst(site, rootObjects);
                case Search.Depth:
                    return resolveSiteDepthFirst(site, rootObjects);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private bool resolveSiteFromAnywhere(IInjectionSite site)
        {
            if (string.IsNullOrEmpty(site.HintPath))
            {
                // if no hint path is given just search from the scene root objects
                return resolveSiteFromRootObjects(site);
            }

            // search from the hint path object
            GameObject objectToSearchFrom = getObjectFromHint(site.HintPath);
            if (objectToSearchFrom == null)
            {
                if (site.Required)
                {
                    Debug.LogError($"Unable to inject dependency for {injectionSiteDebugString(site)} - " +
                                   $"Hint path '{site.HintPath}' did not correspond to a GameObject in the scene " +
                                   $"hierarchy");
                }

                return false;
            }

            return resolveSiteFromObject(site, objectToSearchFrom);
        }

        private IEnumerable<GameObject> ancestors(GameObject gameObject)
        {
            for (var parent = gameObject.transform.parent; parent != null; parent = parent.parent)
            {
                yield return parent.gameObject;
            }
        }

        private bool resolveSiteFromParents(IInjectionSite site, MonoBehaviour script)
        {
            // search for source upwards from script object
            foreach (var gameObject in ancestors(script.gameObject))
            {
                var source = findInjectionSource(site, gameObject);
                if (source != null)
                {
                    site.Resolve(source);
                    return true;
                }
            }

            if (site.Required)
            {
                Debug.LogWarning("Unable to find dependency source in parents");
            }

            return false;
        }

        private bool resolveSiteFromChildren(IInjectionSite site, MonoBehaviour script)
        {
            IEnumerable<GameObject> children;
            switch (site.SearchMethod)
            {
                case Search.Breadth:
                    children = breadthFirstTraversal(new List<GameObject> {script.gameObject});
                    break;
                case Search.Depth:
                    children = depthFirstTraversal(new List<GameObject> {script.gameObject});
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            foreach (var gameObject in children)
            {
                var source = findInjectionSource(site, gameObject);
                if (source != null)
                {
                    site.Resolve(source);
                    return true;
                }
            }

            if (site.Required)
            {
                Debug.LogWarning("Unable to find dependency source in children");
            }

            return false;
        }

        private bool resolveSite(IInjectionSite site, MonoBehaviour script)
        {
            switch (site.FromSource)
            {
                case From.Anywhere:
                    return resolveSiteFromAnywhere(site);
                case From.Parents:
                    return resolveSiteFromParents(site, script);
                case From.Children:
                    return resolveSiteFromChildren(site, script);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void Resolve(MonoBehaviour script)
        {
            var injectionSites = getInjectionSites(script);
            foreach (var injectionSite in injectionSites)
            {
                bool resolved = resolveSite(injectionSite, script);
                if (!resolved && injectionSite.Required)
                {
                    Debug.LogError($"Unable to inject dependency for site {injectionSiteDebugString(injectionSite)}" +
                                   $" - could not find injection source.");
                }
            }
        }

        public void AutoResolve()
        {
            Debug.Log("Auto resolving");
            var autoInjectScripts = Object.FindObjectsOfType<MonoBehaviour>()
                                          .Where(isAutoInjectable);
            foreach (var script in autoInjectScripts)
            {
                Resolve(script);
            }
        }
    }
}
