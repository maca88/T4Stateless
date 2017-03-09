using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using T4Stateless.Tests.Models;

using T4Stateless.Tests.Async;

namespace T4Stateless.Tests
{
    [TestClass]
    public class AsyncSmartphoneTests
    {
        [TestMethod]
        public async Task SmartphoneTest()
        {
            var phone = new Smartphone
            {
                SimInserted = false,
                BatteryLevel = 100
            };
            var events = new List<SmartphoneFireAsyncEvent>();
            var sm = new Async.SmartphoneStateMachine(phone, s => s.State, (s, state) => s.State = state, e =>
            {
                events.Add(e);
                return Task.CompletedTask;
            }, new TestService());

            var expectedEvents = 1;

            await sm.FireBootAsync();
            Assert.AreEqual(SmartphoneState.Idle, phone.State);
            Assert.AreEqual(expectedEvents, events.Count);
            Assert.AreEqual(typeof(SmartphoneFireBootAsyncEvent), events[expectedEvents-1].GetType());
            expectedEvents++;

            await sm.FireCallAsync("123456789");
            Assert.AreEqual(SmartphoneState.Idle, phone.State);
            Assert.AreEqual(expectedEvents, events.Count);
            Assert.AreEqual(typeof(SmartphoneFireCallReentryAsyncEvent), events[expectedEvents-1].GetType());
            Assert.AreEqual("123456789", ((SmartphoneFireCallReentryAsyncEvent)events[expectedEvents-1]).Parameter);
            expectedEvents++;
            phone.SimInserted = true;

            await sm.FireCallAsync("123456789");
            Assert.AreEqual(SmartphoneState.Calling, phone.State);
            Assert.AreEqual(expectedEvents, events.Count);
            Assert.AreEqual(typeof(SmartphoneFireCallAsyncEvent), events[expectedEvents-1].GetType());
            Assert.AreEqual("123456789", ((SmartphoneFireCallAsyncEvent) events[expectedEvents-1]).Parameter);
            expectedEvents++;

            await sm.FireConnectAsync();
            Assert.AreEqual(SmartphoneState.CallConnected, phone.State);
            Assert.AreEqual(expectedEvents, events.Count);
            Assert.AreEqual(typeof(SmartphoneFireConnectAsyncEvent), events[expectedEvents-1].GetType());
            expectedEvents++;

            await sm.FirePlaceOnHoldAsync();
            Assert.AreEqual(SmartphoneState.CallOnHold, phone.State);
            Assert.AreEqual(expectedEvents, events.Count);
            Assert.AreEqual(typeof(SmartphoneFirePlaceOnHoldAsyncEvent), events[expectedEvents-1].GetType());
            expectedEvents++;

            await sm.FireResumeAsync();
            Assert.AreEqual(SmartphoneState.CallConnected, phone.State);
            Assert.AreEqual(expectedEvents, events.Count);
            Assert.AreEqual(typeof(SmartphoneFireResumeAsyncEvent), events[expectedEvents-1].GetType());
            expectedEvents++;

            await sm.FirePlaceOnHoldAsync();
            Assert.AreEqual(SmartphoneState.CallOnHold, phone.State);
            Assert.AreEqual(expectedEvents, events.Count);
            Assert.AreEqual(typeof(SmartphoneFirePlaceOnHoldAsyncEvent), events[expectedEvents-1].GetType());
            expectedEvents++;

            phone.Locked = true;
            await sm.FireResumeAsync();
            Assert.AreEqual(SmartphoneState.CallOnHold, phone.State);
            Assert.AreEqual(expectedEvents, events.Count);
            Assert.AreEqual(typeof(SmartphoneFireResumeReentryAsyncEvent), events[expectedEvents-1].GetType());
        }
    }
}
