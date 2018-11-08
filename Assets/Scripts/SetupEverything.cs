using UnityEngine;
using ServerNS;
public class SetupEverything : MonoBehaviour {
    public static SetupEverything
        instance; //Static instance of GameManager which allows it to be accessed by any other script.

    public ChannelManager receiver;
    //Awake is always called before any Start functions

    public ChannelManager sender;

    //TODO MOVE TO CONNECTION MANAGER
//     Use this for initialization
//    private void Awake() {
//        sender = new ServerNS.Server();
//        receiver = new ServerNS.Server();
//
//        sender.SetupOthererver(receiver);
//        receiver.SetupOthererver(sender);
//
//        if (instance == null)
//
//            //if not, set instance to this
//            instance = this;
//    }
}