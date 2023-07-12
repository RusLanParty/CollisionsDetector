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
            var pedVehs = World.GetNearbyVehicles(player, range).Select(v => new { Vehicle = v, initSpd = v.Speed }).ToList();
            Wait(5);
                foreach (var pedv in pedVehs.Where(v => v.Vehicle.Exists()))
                {
                if (!pedv.Vehicle.IsMotorcycle && pedv.Vehicle.HasCollided)
                {
                    var pedSpeedDif = pedv.initSpd - pedv.Vehicle.Speed;
                    var dmgPed = (int)Math.Abs(pedSpeedDif * pedMultiplier * 4);
                    if (dmgPed >= 1)
                    {
                        Ped dr = pedv.Vehicle.Driver;

                        if (dr != null)
                        {
                            dr.HealthFloat += -1* (dmgPed * Function.Call<float>(Hash.GET_RANDOM_FLOAT_IN_RANGE, 0.7, 1.1));
                        }
                        if (pedv.Vehicle.PassengerCount > 0)
                        {
                            List<Ped> pedPsngrs = new List<Ped>(pedv.Vehicle.Passengers);
                            foreach (Ped pp in pedPsngrs)
                            {
                                if (pp != null)
                                {
                                    pp.HealthFloat += -1 * ((dmgPed * Function.Call<float>(Hash.GET_RANDOM_FLOAT_IN_RANGE, 0.7, 1.1)));
                                }
                            }
                        }
                    }
                    else { dmgPed = 0; }
                    dmgPed = 0;
                }
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



