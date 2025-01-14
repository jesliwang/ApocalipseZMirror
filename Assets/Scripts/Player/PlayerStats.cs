﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Random = UnityEngine.Random;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using FishNet.Transporting;
using FishNet.Connection;

namespace ApocalipseZ
{
    public class PlayerStats : NetworkBehaviour, IStats
    {


        public event Action OnAlteredStats;
        [SyncVar(Channel = Channel.Unreliable, OnChange = nameof(OnSetHealth))]
        public int health;
        [SyncVar(Channel = Channel.Unreliable, OnChange = nameof(OnSetHydratation))]
        public int hydratation = 100;
        public float hydratationSubstractionRate = 3f;
        public int thirstDamage = 1;
        private float hydratationTimer;
        [SyncVar(Channel = Channel.Unreliable, OnChange = nameof(OnSetSatiety))]
        public int satiety = 100;
        public float satietySubstractionRate = 5f;
        public int hungerDamage = 1;
        private float satietyTimer;

        FpsPlayer player;

        public bool Disable;
        private void OnSetHealth(int oldHealth, int newHealth, bool asServer)
        {
            health = newHealth;
            if (health > 0)
            {
                Disable = false;
            }
            OnAlteredStats?.Invoke();
        }
        private void OnSetHydratation(int oldHydratation, int newHydratation, bool asServer)
        {
            hydratation = newHydratation;

            OnAlteredStats?.Invoke();
        }
        private void OnSetSatiety(int oldSatiety, int newSatiety, bool asServer)
        {
            satiety = newSatiety;

            OnAlteredStats?.Invoke();
        }
        private void Start()
        {
            player = GetComponent<FpsPlayer>();
        }
        void Update()
        {
            if (base.IsServer)
            {
                if (Time.time > satietyTimer + satietySubstractionRate)
                {
                    if (satiety <= 0)
                    {
                        satiety = 0;
                        health -= hungerDamage;
                    }

                    satiety -= 1;
                    satietyTimer = Time.time;

                }

                if (Time.time > hydratationTimer + hydratationSubstractionRate)
                {
                    if (hydratation <= 0)
                    {
                        hydratation = 0;
                        health -= thirstDamage;
                    }
                    hydratation -= 1;
                    hydratationTimer = Time.time;
                }

                if (hydratation > 100)
                {
                    hydratation = 100;
                }
                if (satiety > 100)
                {
                    satiety = 100;
                }
            }
            if (!base.IsOwner || Disable)
            {
                return;
            }

            if (IsDead())
            {
                CmdPlayerDeath();
                Disable = true;

            }
            if (transform.position.y < -11.1 && !IsDead())
            {
                CmdTakeDamage(200);
            }
        }

        [ServerRpc(RequireOwnership = false)]
        public void CmdTakeDamage(int damage, NetworkConnection sender = null)
        {
            TakeDamage(damage);
        }
        public bool IsDead()
        {
            return health <= 0;
        }

        public void AddHealth(int life)
        {
            health += life;

            if (health > 100)
            {
                health = 100;
            }
            OnAlteredStats?.Invoke();
        }
        public void TakeDamage(int damage)
        {
            health -= damage;

            if (health < 0)
            {
                health = 0;
            }
            OnAlteredStats?.Invoke();
        }
        private void PlayerDeath()
        {
            player.DroppAllItems();
            StartCoroutine(Respawn());
        }
        private IEnumerator Respawn()
        {
            yield return new WaitForSeconds(5f);
            AddHealth(200);
            AddHydratation(100);
            AddSatiety(100);
            GetComponent<FpsPlayer>().TargetRespaw();
            yield break;
        }
        [ServerRpc(RequireOwnership = false)]
        public void CmdPlayerDeath(NetworkConnection sender = null)
        {
            PlayerDeath();
            // sender.identity.GetComponent<FpsPlayer>().GetWeaponManager().TargetDesEquipWeapon(opponentIdentity.connectionToClient);
        }

        public float GetDamage()
        {
            throw new NotImplementedException();
        }

        public void AddSatiety(int points)
        {
            satiety += points;
        }

        public void AddHydratation(int points)
        {
            hydratation += points;
        }
    }
}