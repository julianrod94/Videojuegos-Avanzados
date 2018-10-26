using ServerNS;
using UnityEngine;

public class SetupEverything : MonoBehaviour {
    public static SetupEverything
        instance; //Static instance of GameManager which allows it to be accessed by any other script.

    public Server receiver;
    //Awake is always called before any Start functions

    public Server sender;

    // Use this for initialization
    private void Awake() {
        sender = new Server();
        receiver = new Server();

        sender.SetupOtherServer(receiver);
        receiver.SetupOtherServer(sender);

        if (instance == null)

            //if not, set instance to this
            instance = this;
    }
}