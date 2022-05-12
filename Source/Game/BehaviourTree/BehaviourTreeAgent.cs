using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BehaviourTree
{
    public interface IBehaviourTreeAgent
    {
        Blackboard GetBlackboard();
    }
}
