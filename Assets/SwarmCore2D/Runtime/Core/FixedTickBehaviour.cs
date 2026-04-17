using SwarmCore2D.ScriptableObjects;
using UnityEngine;

namespace SwarmCore2D.Core
{
    public class FixedTickBehaviour : MonoBehaviour
    {
        [SerializeField] SwarmProfile profile;

        private FixedTickRunner runner;

        public FixedTickRunner Runner => runner;

        void Awake()
        {
            float rate = (profile != null) ? profile.tickRate : 60f;
            runner = new FixedTickRunner(rate);
        }

        void Update()
        {
            runner.Update(Time.deltaTime);
        }
    }
}
