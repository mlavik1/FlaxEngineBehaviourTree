namespace BehaviourTree
{
    [System.AttributeUsage(System.AttributeTargets.Class)]
    public class BTDecoratorAttribute : System.Attribute
    {
        public string name;
        public string description;

        public BTDecoratorAttribute(string name = "", string description = "")
        {
            this.name = name;
            this.description = description;
        }
    }
}
