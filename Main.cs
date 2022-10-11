﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using GTA;
using GTA.Math;
using GTA.Native;
using GTA.UI;
using static System.Net.Mime.MediaTypeNames;

namespace First
{
    public class Main : Script
    {
        float speed;
        float afterSpeed;
        float speedDif = 0;
        float dmg;
        int time;
        int nowTime;
        int res;
        float pedSpeed;
        float pedAfterSpeed;
        float pedSpeedDif;
        bool update;
        float dmgPed;
        float pedMultiplier;
        float playerMultiplier;



        public Main()
        {
            playerMultiplier = Settings.GetValue("SETTINGS", "playerDmgMult", 2.0f);
            pedMultiplier = Settings.GetValue("SETTINGS", "pedDmgMult", 1.0f);
            update = true;
            res = 0;
            Tick += onTick;

        }
        

        public void PedDam()
        {
            List<Ped> pedes = new List<Ped>(World.GetNearbyPeds(Game.Player.Character, 1000f));
            List<Vehicle> pedVehs = new List<Vehicle>();

            foreach (Ped pedik in pedes)
            {
                if (pedik.IsInVehicle())
                {
                    Vehicle pedikVeh = pedik.CurrentVehicle;
                    pedVehs.Add(pedikVeh);
                }
            }
           
                foreach (Vehicle pedv in pedVehs)
                {
                if (pedv.HasCollided)
                {
                        pedSpeed = pedv.Speed;
                         Wait(1);
                        pedAfterSpeed = pedv.Speed;
                        pedSpeedDif = pedSpeed - pedAfterSpeed;
                        dmgPed = pedSpeedDif * pedMultiplier;
                        if (dmgPed <= 0)
                        {
                            dmgPed = dmgPed * -1;
                        }


                    if (dmgPed > 0)
                    {
                       

                        if (pedv.Exists())
                        {

                            Ped dr = pedv.Driver;

                            if (dr != null)
                            {
                                dr.ApplyDamage((int)dmgPed);
                                //GTA.UI.Screen.ShowHelpText("Damage: " + dmgPed + "Mult: " + pedMultiplier, 3000, true, false);
                            }
                            if (pedv.PassengerCount > 0)
                            {
                                List<Ped> pedPsngrs = new List<Ped>(pedv.Passengers);
                                foreach (Ped pp in pedPsngrs)
                                {
                                    if (pp != null)
                                    {
                                        pp.ApplyDamage((int)dmgPed);
                                    }
                                }
                            }
                        }
                    }
                    else { dmgPed = 0; }

                }
                dmgPed = 0;

            }
                    }

            public void onTick(object sender, EventArgs e)
            {
                PedDam();
            
            Ped playerPed = Game.Player.Character;
                Vehicle car = playerPed.CurrentVehicle;

            if (playerPed.IsInVehicle() && (update))
            {
                speed = car.Speed;
                time = Game.GameTime;
                update = false;
            }
                if (playerPed.IsInVehicle())
            {
                nowTime = Game.GameTime;
                res = nowTime - time;
                if (res > 2)
                {
                    update = true;
                    
                }
                if (car.HasCollided)
                {
                    afterSpeed = car.Speed;
                    speedDif = speed - afterSpeed;
                }
                if (speedDif > 0)
                {
                    dmg = speedDif * playerMultiplier;
                    playerPed.ApplyDamage((int)dmg);
                    //GTA.UI.Screen.ShowSubtitle("Damage: " + dmg + "Mult:" + playerMultiplier, 3000);
                    speedDif = 0;
                }
            }
                
            }

           
        }
    }



