using System;
using System.Collections.Generic;
using System.Reflection;
using SylhShrinkerCartPlus.Config;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace SylhShrinkerCartPlus.Utils
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
        
        public static void TryMarkForExecution(PhysGrabObject obj)
        {
            if (obj == null || !SemiFunc.IsMasterClientOrSingleplayer())
                return;

            // Vérifie isEnemy
            var isEnemyField = typeof(PhysGrabObject).GetField("isEnemy", BindingFlags.NonPublic | BindingFlags.Instance);
            if (isEnemyField == null || !(bool)isEnemyField.GetValue(obj))
                return;

            // enemyRigidbody
            var enemyRbField = typeof(PhysGrabObject).GetField("enemyRigidbody", BindingFlags.NonPublic | BindingFlags.Instance);
            var enemyRb = enemyRbField?.GetValue(obj);
            if (enemyRb == null) return;

            // enemy
            var enemyField = enemyRb.GetType().GetField("enemy", BindingFlags.Public | BindingFlags.Instance);
            var enemy = enemyField?.GetValue(enemyRb);
            if (enemy == null || enemiesToKill.Contains(enemy))
                return;
            
            LogWrapper.Info($"[ExecutionCheck] Found isEnemy object: '{obj.name}', checking further eligibility...");

            // HasHealth + Health
            var hasHealthField = enemy.GetType().GetField("HasHealth", BindingFlags.NonPublic | BindingFlags.Instance);
            var healthField = enemy.GetType().GetField("Health", BindingFlags.NonPublic | BindingFlags.Instance);
            if (!(bool)(hasHealthField?.GetValue(enemy))) return;
            var health = healthField?.GetValue(enemy);
            if (health == null) return;

            // Déjà mort ?
            var deadField = health.GetType().GetField("dead", BindingFlags.NonPublic | BindingFlags.Instance);
            if ((bool)(deadField?.GetValue(health))) return;

            // healthCurrent
            var healthCurrentField = health.GetType().GetField("healthCurrent", BindingFlags.NonPublic | BindingFlags.Instance);
            int hp = (int)(healthCurrentField?.GetValue(health) ?? 0);

            // Récupère l’event onDeath
            var onDeathField = health.GetType().GetField("onDeath", BindingFlags.Public | BindingFlags.Instance);
            if (onDeathField == null) return;
            var onDeathEvent = onDeathField.GetValue(health) as UnityEvent;
            if (onDeathEvent == null) return;

            // Ajoute à la liste
            enemiesToKill.Add(enemy);
            LogWrapper.Info($"[ExecutionManager] Marked enemy for execution: {enemy.GetType().Name}");

            // Abonnement au onDeath
            UnityAction deathHandler = null;
            deathHandler = () =>
            {
                LogWrapper.Info($"[ExecutionManager] Enemy has died. Removing from kill list.");
                enemiesToKill.Remove(enemy);
                onDeathEvent.RemoveListener(deathHandler);
            };
            onDeathEvent.AddListener(deathHandler);

            // Appelle Hurt pour tuer proprement
            var hurtMethod = health.GetType().GetMethod("Hurt", BindingFlags.Public | BindingFlags.Instance);
            hurtMethod?.Invoke(health, new object[] { hp + 1, Vector3.up });
        }

        public static bool IsEnemyMarked(object enemy) => enemiesToKill.Contains(enemy);
    }
}