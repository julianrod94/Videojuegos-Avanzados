using System;
using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

public class BitbufferTest {

    private int[] numbers = { 2, 4 };

    [Test]
    public void SendBoolTest() {
        Bitbuffer bitbuffer = new Bitbuffer();
        bitbuffer.WriteBool(true);
        bitbuffer.WriteBool(false);
        Assert.True(bitbuffer.ReadBit());
        Assert.False(bitbuffer.ReadBit());
    }

    [Test]
    public void SendIntsTest() {
        Bitbuffer bitbuffer = new Bitbuffer();
        for (int i = 0; i < numbers.Length; i++) {
            bitbuffer.WriteInt(numbers[i], -5, 3356748);
            Assert.AreEqual(numbers[i], bitbuffer.ReadInt(-5, 3356748));
            bitbuffer.flush();
        }
    }

    [Test]
    public void SendIntsInSucesion() {
        Bitbuffer bitbuffer = new Bitbuffer();
        for (int i = 0; i < numbers.Length; i++) {
            bitbuffer.WriteInt(numbers[i], 0, 5);
        }      
        for (int i = 0; i < numbers.Length; i++) {
            Assert.AreEqual(numbers[i], bitbuffer.ReadInt(0, 5));
        }
    }
    
    [Test]
    public void SendIntsInSucesionnegative() {
        Bitbuffer bitbuffer = new Bitbuffer();
        for (int i = -20; i < 20; i++) {
            bitbuffer.WriteInt(i, -20, 20);
        }
        
        bitbuffer.toRead();
        for (int i = -20; i < 20; i++) {
            Assert.AreEqual(i, bitbuffer.ReadInt(-20, 20));
        }
    }
    
    [Test]
    public void SendIntsInStepsNegative() {
        Bitbuffer bitbuffer = new Bitbuffer();
        for (int i = -1000; i < 1000; i+=3) {
            bitbuffer.WriteInt(i, -1000, 1000);
        }
        
        bitbuffer.toRead();
        for (int i = -1000; i < 1000; i+=3) {
            Assert.AreEqual(i, bitbuffer.ReadInt(-1000, 1000));
        }
    }

    // A UnityTest behaves like a coroutine in PlayMode
    // and allows you to yield null to skip a frame in EditMode
    [UnityTest]
    public IEnumerator BitbufferTestWithEnumeratorPasses() {
        // Use the Assert class to test conditions.
        // yield to skip a frame
        yield return null;
    }
}
