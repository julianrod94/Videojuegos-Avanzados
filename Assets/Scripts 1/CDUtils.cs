using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class CDUtils {
    public static void Delay(float ms, Action toDo) {
        new Thread(() => { Thread.Sleep((int)ms); toDo(); }).Start();
    }
}
