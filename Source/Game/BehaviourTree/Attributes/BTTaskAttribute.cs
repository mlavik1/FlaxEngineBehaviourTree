namespace BehaviourTree
{
    [System.AttributeUsage(System.AttributeTargets.Class)]
    public class BTTaskAttribute : System.Attribute
    {
        public string name;
        public string description;

        public BTTaskAttribute(string name = "", string description = "")
        {
            this.name = name;
            this.description = description;
        }
    }
}
