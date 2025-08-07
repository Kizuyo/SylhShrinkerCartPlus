using System.Reflection;
using SylhShrinkerCartPlus.Components;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace SylhShrinkerCartPlus.Utils.Cheat.Enemy
{
    public static class EnemyExecutionManager
    {
        private static readonly HashSet<object> enemiesToKill = new();

        static EnemyExecutionManager()
        {
            // Nettoie la liste quand une nouvelle scène est chargée
            SceneManager.sceneLoaded += (_, _) =>
            {
                enemiesToKill.Clear();
                Plugin.Log.LogInfo("[ExecutionManager] Cleared enemiesToKill list on scene load.");
            };
        }

        public static class EnemyReflectionUtils
        {
            public static bool TryGetEnemyComponents(
                ShrinkableTracker tracker,
                out object enemy,
                out object health,
                out int currentHp,
                out UnityEvent onDeathEvent)
            {
                enemy = null;
                health = null;
                currentHp = 0;
                onDeathEvent = null;

                PhysGrabObject obj = tracker.GrabObject;

                if (obj == null || !SemiFunc.IsMasterClientOrSingleplayer())
                {
                    return false;
                }

                if (!FastReflection.TryGetField(
                        typeof(PhysGrabObject),
                        obj,
                        "isEnemy",
                        out bool isEnemy
                    ) || !isEnemy)
                {
                    return false;
                }

                if (!FastReflection.TryGetField(
                        typeof(PhysGrabObject),
                        obj,
                        "enemyRigidbody",
                        out object enemyRb
                    ) || enemyRb == null)
                {
                    return false;
                }

                if (!FastReflection.TryGetField(
                        enemyRb.GetType(),
                        enemyRb, "enemy",
                        out enemy,
                        BindingFlags.Public | BindingFlags.Instance
                    ) || enemy == null
                   )
                {
                    return false;
                }

                if (!FastReflection.TryGetField(
                        enemy.GetType(),
                        enemy,
                        "HasHealth",
                        out bool hasHealth
                    ) || !hasHealth)
                {
                    return false;
                }

                if (!FastReflection.TryGetField(
                        enemy.GetType(),
                        enemy,
                        "Health",
                        out health
                    ) || health == null)
                {
                    return false;
                }

                if (!FastReflection.TryGetField(
                        health.GetType(),
                        health,
                        "dead",
                        out bool isDead
                    ) || isDead)
                {
                    return false;
                }

                if (!FastReflection.TryGetField(
                        health.GetType(),
                        health,
                        "healthCurrent",
                        out currentHp
                    ))
                {
                    return false;
                }

                if (!FastReflection.TryGetField(
                        health.GetType(),
                        health,
                        "onDeath",
                        out onDeathEvent,
                        BindingFlags.Public | BindingFlags.Instance
                    ) || onDeathEvent == null)
                {
                    return false;
                }

                return true;
            }
        }

        public static void TryMarkForExecution(ShrinkableTracker tracker)
        {
            if (!EnemyReflectionUtils.TryGetEnemyComponents(tracker, out var enemy, out var health, out int hp,
                    out var onDeathEvent))
            {
                return;
            }

            if (enemiesToKill.Contains(enemy))
            {
                return;
            }

            enemiesToKill.Add(enemy);

            UnityAction deathHandler = null;
            deathHandler = () =>
            {
                enemiesToKill.Remove(enemy);
                onDeathEvent.RemoveListener(deathHandler);
            };
            onDeathEvent.AddListener(deathHandler);

            FastReflection.TryInvokeMethod(
                health.GetType(),
                health,
                "Hurt",
                new object[] { hp + 1, Vector3.up },
                BindingFlags.Public | BindingFlags.Instance
            );
        }
    }
}