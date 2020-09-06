using System;

namespace Torii.Injection
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class InjectAttribute : Attribute
    {
        public From From { get; }
        public Search Search { get; }

        public string HintPath { get; }

        public bool Required { get; }

        public InjectAttribute(From from = From.Parents,
            Search search = Search.Breadth,
            string hintPath = null,
            bool required = true)
        {
            From = from;
            Search = search;
            HintPath = hintPath;
            Required = required;
        }
    }

    public enum From
    {
        Parents,
        Children,
        Anywhere
    }

    public enum Search
    {
        Breadth,
        Depth
    }
}
