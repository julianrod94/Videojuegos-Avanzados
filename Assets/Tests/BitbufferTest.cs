﻿using System;
using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using ChannelNS;
using InputManagerNS;

public class BitbufferTest {

    private int[] numbers = { 2, 4 };
    private float epsilon = 1E-4f;

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
            bitbuffer.Flush();
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
        
        bitbuffer.ToRead();
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
        
        bitbuffer.ToRead();
        for (int i = -1000; i < 1000; i+=3) {
            Assert.AreEqual(i, bitbuffer.ReadInt(-1000, 1000));
        }
    }
    
    [Test]
    public void SendFloats() {
        Bitbuffer bitbuffer = new Bitbuffer();
        for (float i = 0; i < 10; i+=0.1f) {
            bitbuffer.WriteFloat(i, 0, 10, 0.1f);
        }
        
        bitbuffer.ToRead();
        for (float i = 0; i < 10; i+=0.1f) {
            float red = bitbuffer.ReadFloat(0, 10, 0.1f);
            Assert.Less(Mathf.Abs(i-red), epsilon);
        }
    }
    
        
    [Test]
    public void SendFloatsNegative() {
        Bitbuffer bitbuffer = new Bitbuffer();
        for (float i = -50; i < 55; i+=2.1f) {
            bitbuffer.WriteFloat(i, -50, 55, 2.1f);
        }
        
        bitbuffer.ToRead();
        for (float i = -50; i < 55; i+=2.1f) {
            Assert.Less(Mathf.Abs(i-bitbuffer.ReadFloat(-50, 55, 2.1f)), epsilon);
        }
    }
    
    private readonly int _minInputs = 0;
    private readonly int _maxInputs = 255;
        
    private readonly int _minInputNumber = 0;
    private readonly int _maxInputNumber = 1024*8-1;
        
    private readonly float _minDt = 0;
    private readonly float _maxDT = 1f;
    private readonly float _stepDT = 1/100f;

    private readonly int _inputCommands = Enum.GetValues(typeof(InputEnum)).Length;

    
    [Test]
    public void inputs() {
        var inputM = new InputManager();
        Bitbuffer buffer = new Bitbuffer();
        var data = new List<PlayerAction>();
        
        data.Add(new PlayerAction(InputEnum.Left, 1, 0.2f, Quaternion.identity));
        data.Add(new PlayerAction(InputEnum.Right, 2, 0.2f, Quaternion.identity));
        data.Add(new PlayerAction(InputEnum.Forward, 3, 0.2f, Quaternion.identity));
        
        buffer.ToWrite();
        buffer.WriteInt(data.Count, _minInputs, _maxInputs);
        foreach (var playerAction in data) {
            buffer.WriteInt((int)playerAction.inputCommand, 0, _inputCommands);
            buffer.WriteInt(playerAction.inputNumber, _minInputNumber, _maxInputNumber);
            buffer.WriteFloatRounded(playerAction.deltaTime, _minDt, _maxDT, _stepDT);
        }
        
        buffer.LoadBytes(buffer.GenerateBytes());
        buffer.ToRead();
        List<PlayerAction> actions;

        var amount = buffer.ReadInt(_minInputs, _maxInputs);
        actions = new List<PlayerAction>(amount);
        for (int i = 0; i < amount; i++) {
            var code = (InputEnum)buffer.ReadInt(0, _inputCommands);
            var number = buffer.ReadInt(_minInputNumber, _maxInputNumber);
            var dT = buffer.ReadFloat(_minDt, _maxDT, _stepDT);
            actions.Add(new PlayerAction(code, number, dT, Quaternion.identity));
        }
        
        actions.ForEach((a) => Debug.Log(a.inputCommand));
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
