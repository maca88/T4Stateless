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
                SimInserted = true,
                BatteryLevel = 100
            };
            var events = new List<SmartphoneFireBaseEvent>();
            var sm = new SmartphoneStateMachine(phone, s => s.State, (s, state) => s.State = state, e =>
            {
                events.Add(e);
            });

            sm.FireBoot();
            Assert.AreEqual(SmartphoneState.Idle, phone.State);
            Assert.AreEqual(1, events.Count);
            Assert.AreEqual(typeof(SmartphoneFireBootEvent), events[0].GetType());

            sm.FireCall("123456789");
            Assert.AreEqual(SmartphoneState.Calling, phone.State);
            Assert.AreEqual(2, events.Count);
            Assert.AreEqual(typeof(SmartphoneFireCallEvent), events[1].GetType());
            Assert.AreEqual("123456789", ((SmartphoneFireCallEvent) events[1]).Parameter);

            sm.FireConnect();
            Assert.AreEqual(SmartphoneState.CallConnected, phone.State);
            Assert.AreEqual(3, events.Count);
            Assert.AreEqual(typeof(SmartphoneFireConnectEvent), events[2].GetType());

            sm.FirePlaceOnHold();
            Assert.AreEqual(SmartphoneState.CallOnHold, phone.State);
            Assert.AreEqual(4, events.Count);
            Assert.AreEqual(typeof(SmartphoneFirePlaceOnHoldEvent), events[3].GetType());

            sm.FireResume();
            Assert.AreEqual(SmartphoneState.CallConnected, phone.State);
            Assert.AreEqual(5, events.Count);
            Assert.AreEqual(typeof(SmartphoneFireResumeEvent), events[4].GetType());

            sm.FirePlaceOnHold();
            Assert.AreEqual(SmartphoneState.CallOnHold, phone.State);
            Assert.AreEqual(6, events.Count);
            Assert.AreEqual(typeof(SmartphoneFirePlaceOnHoldEvent), events[5].GetType());

            phone.Locked = true;
            sm.FireResume();
            Assert.AreEqual(SmartphoneState.CallOnHold, phone.State);
            Assert.AreEqual(7, events.Count);
            Assert.AreEqual(typeof(SmartphoneFireResumeReentryEvent), events[6].GetType());

        }
    }
}
