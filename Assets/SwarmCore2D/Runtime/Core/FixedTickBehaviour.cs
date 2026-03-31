using UnityEngine;

namespace SwarmCore2D.Core
{
    public class FixedTickBehaviour : MonoBehaviour
    {
        public float tickRate = 60f;

        private FixedTickRunner runner;

        public FixedTickRunner Runner => runner;

        void Awake()
        {
            runner = new FixedTickRunner(tickRate);
        }

        void Update()
        {
            runner.Update(Time.deltaTime);
        }
    }
}
