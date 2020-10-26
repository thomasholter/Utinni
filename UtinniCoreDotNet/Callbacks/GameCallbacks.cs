﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace UtinniCoreDotNet.Callbacks
{
    public static class GameCallbacks
    {
        private static readonly SynchronizedCollection<Action> installCallbacks = new SynchronizedCollection<Action>();
        private static readonly SynchronizedCollection<Action> setupSceneCallbacks = new SynchronizedCollection<Action>();
        private static readonly SynchronizedCollection<Action> cleanupSceneCallbacks = new SynchronizedCollection<Action>();
        private static readonly ConcurrentQueue<Action> preMainLoopCallQueue = new ConcurrentQueue<Action>();
        private static readonly ConcurrentQueue<Action> mainLoopCallQueue = new ConcurrentQueue<Action>();

        private static UtinniCore.Delegates.Action_ callInstallCallbacksAction;
        private static UtinniCore.Delegates.Action_ callSetupSceneCallbacksAction;
        private static UtinniCore.Delegates.Action_ callCleanupSceneCallbacksAction;
        private static UtinniCore.Delegates.Action_ dequeuePreMainLoopCallsAction;
        private static UtinniCore.Delegates.Action_ dequeueMainLoopCallsAction;
        public static void Initialize()
        {
            // Storing this in a variable is somehow needed to prevent corruption on WinForms resize. Very odd bug that I still don't fully understand.
            callInstallCallbacksAction = CallInstallCallbacks;
            callSetupSceneCallbacksAction = CallSetupSceneCallbacks;
            callCleanupSceneCallbacksAction = CallCleanupSceneCallbacks;
            dequeuePreMainLoopCallsAction = DequeuePreMainLoopCalls;
            dequeueMainLoopCallsAction = DequeueMainLoopCalls;

            UtinniCore.Utinni.Game.AddInstallCallback(callInstallCallbacksAction);
            UtinniCore.Utinni.Game.AddSetSceneCallback(callSetupSceneCallbacksAction);
            UtinniCore.Utinni.Game.AddCleanupSceneCallback(callCleanupSceneCallbacksAction);
            UtinniCore.Utinni.Game.AddPreMainLoopCallback(dequeuePreMainLoopCallsAction);
            UtinniCore.Utinni.Game.AddMainLoopCallback(dequeueMainLoopCallsAction);
        }

        public static void AddInstallCallback(Action call)
        {
            installCallbacks.Add(call);
        }

        public static void AddSetupSceneCall(Action call)
        {
            setupSceneCallbacks.Add(call);
        }

        public static void AddCleanupSceneCall(Action call)
        {
            cleanupSceneCallbacks.Add(call);
        }

        public static void AddPreMainLoopCall(Action call)
        {
            preMainLoopCallQueue.Enqueue(call);
        }

        public static void AddMainLoopCall(Action call)
        {
            mainLoopCallQueue.Enqueue(call);
        }

        public static void RemoveInstallCallback(Action call)
        {
            installCallbacks.Remove(call);
        }

        public static void RemoveSetupSceneCall(Action call)
        {
            setupSceneCallbacks.Remove(call);
        }

        public static void RemoveCleanupSceneCall(Action call)
        {
            cleanupSceneCallbacks.Remove(call);
        }

        private static void DequeuePreMainLoopCalls()
        {
            while (preMainLoopCallQueue.Count > 0)
            {
                if (preMainLoopCallQueue.TryDequeue(out var func))
                {
                    func();
                }
            }
        }

        private static void DequeueMainLoopCalls()
        {
            while (mainLoopCallQueue.Count > 0)
            {
                if (mainLoopCallQueue.TryDequeue(out var func))
                {
                    func();
                }
            }
        }

        private static void CallInstallCallbacks()
        {
            foreach (Action callback in installCallbacks)
            {
                callback();
            }
        }

        private static void CallSetupSceneCallbacks()
        {
            foreach (Action callback in setupSceneCallbacks)
            {
                callback();
            }
        }

        private static void CallCleanupSceneCallbacks()
        {
            foreach (Action callback in cleanupSceneCallbacks)
            {
                callback();
            }
        }
    }
}