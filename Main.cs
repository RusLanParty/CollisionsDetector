using System;
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
        bool showDebug = false;
        public int count;
        public float speed;
        public float afterSpeed;
        public float speedDif = 0;
        public float dmg;
        public int health;
        public int time;
        public int nowTime;
        public int res;
        public int time1;
        public int nowTime1;
        public int res1;
        public float dmgp;
        public float pedSpeed;
        public float pedAfterSpeed;
        public float pedSpeedDif;
        public bool update1;
        public bool update;
        public float dmgPed;
        public int i;


        public Main()
        {
            update = true;
            update1 = true;
            res1 = 0;
            res = 0;
            Tick += onTick;
            KeyDown += onKeyDown;

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
                        dmgPed = pedSpeedDif * 0.5f;
                        if (dmgPed <= 0)
                        {
                            dmgPed = dmgPed * -1;
                        }


                    if (dmgPed > 0)
                    {
                        if (dmgPed <= 30) { dmgPed = dmgPed * 5; }
                        if (dmgPed > 30) { dmgPed = dmgPed * 3; }
                        

                        if (pedv.Exists())
                        {

                            Ped dr = pedv.Driver;

                            if (dr != null)
                            {
                                dr.ApplyDamage((int)dmgPed);
                                if (showDebug) { GTA.UI.Screen.ShowHelpText("There was a collision! ~r~Damage: ~y~" + dmgPed, 3000, true, false); }
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
                    dmg = speedDif * 0.8f;
                    playerPed.ApplyDamage((int)dmg);
                    speedDif = 0;
                }
            }
                
            }

            private void onKeyDown(object sender, KeyEventArgs e)
            {
                if (e.KeyCode == Keys.NumPad9)
                {
                    showDebug = !showDebug;
                GTA.UI.Screen.ShowSubtitle("Debug: " + showDebug, 1000);
                }



            }
        }
    }



