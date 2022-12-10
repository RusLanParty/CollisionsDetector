using System;
using System.Collections.Generic;
using System.Dynamic;
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
        bool update;
        float pedMultiplier;
        float playerMultiplier;
        bool playerEnabled;
        bool pedsEnabled;
        float range;

        public Main()
        {
            playerEnabled = Settings.GetValue("SETTINGS", "playerEnabled", true);
            pedsEnabled = Settings.GetValue("SETTINGS", "pedsEnabled", true);
            playerMultiplier = Settings.GetValue("SETTINGS", "playerDmgMult", 1.0f);
            pedMultiplier = Settings.GetValue("SETTINGS", "pedDmgMult", 1.0f);
            range = Settings.GetValue("SETTINGS", "range", 50.0f);
            update = true;
            res = 0;
            Tick += onTick;

        }
        
        public void pedDamage()
        {
            Ped player = Game.Player.Character;
            var pedVehs = World.GetNearbyVehicles(player, range).Where(v => v.HasCollided).Select(v => new { Vehicle = v, initSpd = v.Speed }).ToList();
            Wait(0);
            foreach(var pedv in pedVehs.Where(v => v.Vehicle.Exists()))
            {
                var pedSpeedDif = pedv.initSpd - pedv.Vehicle.Speed;
                var dmgPed = (int)Math.Abs(pedSpeedDif * pedMultiplier * 5);
                if (dmgPed >= 0)
                {
                        Ped dr = pedv.Vehicle.Driver;

                        if (dr != null)
                        {
                            dr.ApplyDamage(dmgPed);
                            //GTA.UI.Screen.ShowHelpText("Damage: " + dmgPed + " Range: " + range + " initSpd=" + pedv.initSpd, 3000, true, false);
                        }
                        if (pedv.Vehicle.PassengerCount > 0)
                        {
                            List<Ped> pedPsngrs = new List<Ped>(pedv.Vehicle.Passengers);
                            foreach (Ped pp in pedPsngrs)
                            {
                                if (pp != null)
                                {
                                    pp.ApplyDamage((int)dmgPed);
                                }
                            }
                        }
                }

               else { dmgPed = 0; }
                
                dmgPed = 0;

            }
                    }
        public void playerDamage()
        {
            Ped playerPed = Game.Player.Character;
            Vehicle car = playerPed.CurrentVehicle;

            if (playerPed.IsInVehicle() && update)
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

        public void onTick(object sender, EventArgs e)
        {
            if (pedsEnabled) { pedDamage(); }
            if (playerEnabled) { playerDamage(); }
   
        }
        }
    }



