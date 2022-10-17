using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using uOSC;
using EVMC4U;

namespace Dollars
{
    public class MoCapManager : MonoBehaviour
    {
        public int ListenOnPort = 39539;
        uOscServer os;
        ExternalReceiver re;
        // Start is called before the first frame update
        void Start()
        {
            os = gameObject.AddComponent<uOscServer>();
            os.enabled = false;
            os.port = ListenOnPort;
            os.enabled = true;
            re = gameObject.AddComponent<ExternalReceiver>();
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
