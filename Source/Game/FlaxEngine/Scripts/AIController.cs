using FlaxEngine;

namespace BehaviourTree
{
    public class AIController : Script
    {
        public BehaviourTree behaviourTree;

        public override void OnStart()
        {
            CompositeNode comp1 = new CompositeNode(CompositeType.Sequence);
            DecoratorNode dec1 = new DecoratorNode(new IsPastTimeDecorator() { TimeToWait = 5.0f, InverseCondition = true });;
            comp1.decorators.Add(dec1);
            DelayTask task1 = new DelayTask() { DelayInSeconds = 1.0f };
            PrintTask task2 = new PrintTask() { StringToPrint = "Hello, World!" };
            comp1.AddChild(new TaskNode(task1));
            comp1.AddChild(new TaskNode(task2));

            behaviourTree = new BehaviourTree();
            behaviourTree.SetRootNode(comp1);
        }
        
        public override void OnUpdate()
        {
            behaviourTree.Update();
        }
    }
}
