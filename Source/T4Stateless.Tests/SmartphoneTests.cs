using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using T4Stateless.Tests.Models;

namespace T4Stateless.Tests
{
    [TestClass]
    public class SmartphoneTests
    {
        [TestMethod]
        public void SmartphoneTest()
        {
            var phone = new Smartphone
            {
                SimInserted = false,
                BatteryLevel = 100
            };
            var events = new List<SmartphoneFireEvent>();
            var sm = new SmartphoneStateMachine(phone, s => s.State, (s, state) => s.State = state, e =>
            {
                events.Add(e);
            }, new TestService());

            var expectedEvents = 1;

            sm.FireBoot();
            Assert.AreEqual(SmartphoneState.Idle, phone.State);
            Assert.AreEqual(expectedEvents, events.Count);
            Assert.AreEqual(typeof(SmartphoneFireBootEvent), events[expectedEvents-1].GetType());

            sm.FireCall("123456789"); // ignore battery > 90
            Assert.AreEqual(SmartphoneState.Idle, phone.State);
            Assert.AreEqual(expectedEvents, events.Count);
            phone.BatteryLevel = 80;

            sm.FireCall("123456789"); // ignore on default
            Assert.AreEqual(SmartphoneState.Idle, phone.State);
            Assert.AreEqual(expectedEvents, events.Count);
            expectedEvents++;
            phone.SimInserted = true;

            sm.FireCall("123456789");
            sm.FireCall("12345");
            Assert.AreEqual(SmartphoneState.Calling, phone.State);
            Assert.AreEqual(expectedEvents, events.Count);
            Assert.AreEqual(typeof(SmartphoneFireCallEvent), events[expectedEvents-1].GetType());
            Assert.AreEqual("123456789", ((SmartphoneFireCallEvent) events[expectedEvents-1]).Parameter);
            expectedEvents++;

            sm.FireConnect();
            Assert.AreEqual(SmartphoneState.CallConnected, phone.State);
            Assert.AreEqual(expectedEvents, events.Count);
            Assert.AreEqual(typeof(SmartphoneFireConnectEvent), events[expectedEvents-1].GetType());
            expectedEvents++;

            sm.FirePlaceOnHold();
            Assert.AreEqual(SmartphoneState.CallOnHold, phone.State);
            Assert.AreEqual(expectedEvents, events.Count);
            Assert.AreEqual(typeof(SmartphoneFirePlaceOnHoldEvent), events[expectedEvents-1].GetType());
            expectedEvents++;

            sm.FireResume();
            Assert.AreEqual(SmartphoneState.CallConnected, phone.State);
            Assert.AreEqual(expectedEvents, events.Count);
            Assert.AreEqual(typeof(SmartphoneFireResumeEvent), events[expectedEvents-1].GetType());
            expectedEvents++;

            sm.FirePlaceOnHold();
            Assert.AreEqual(SmartphoneState.CallOnHold, phone.State);
            Assert.AreEqual(expectedEvents, events.Count);
            Assert.AreEqual(typeof(SmartphoneFirePlaceOnHoldEvent), events[expectedEvents-1].GetType());
            expectedEvents++;

            phone.Locked = true;
            sm.FireResume();
            Assert.AreEqual(SmartphoneState.CallOnHold, phone.State);
            Assert.AreEqual(expectedEvents, events.Count);
            Assert.AreEqual(typeof(SmartphoneFireResumeReentryEvent), events[expectedEvents-1].GetType());
        }
    }
}
