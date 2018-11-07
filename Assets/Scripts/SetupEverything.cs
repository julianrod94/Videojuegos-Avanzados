using UnityEngine;

public class SetupEverything : MonoBehaviour {
    public static SetupEverything
        instance; //Static instance of GameManager which allows it to be accessed by any other script.

    public ChannelManager receiver;
    //Awake is always called before any Start functions

    public ChannelManager sender;

    //TODO MOVE TO CONNECTION MANAGER
    // Use this for initialization
//    private void Awake() {
//        sender = new ChannelManager();
//        receiver = new ChannelManager();
//
//        sender.SetupOtherServer(receiver);
//        receiver.SetupOtherServer(sender);
//
//        if (instance == null)
//
//            //if not, set instance to this
//            instance = this;
//    }
}