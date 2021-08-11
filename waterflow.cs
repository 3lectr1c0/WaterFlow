using System.Collections.Generic;
using Oxide.Core.Libraries.Covalence;

namespace Oxide.Plugins
{
    [Info("WaterFlow", "3LECTR1C", "0.1.2")]
    [Description("Ocean related commands for changing the water level")]
    public class waterflow : CovalencePlugin
    {
        private Timer ftimer;


        protected override void LoadDefaultMessages()  
        {
            lang.RegisterMessages(new Dictionary<string, string>
            {
                ["FloodStart"] = "The map is now flooding.",
                ["FloodDown"] = "The water is receding.",
                ["FloodStop"] = "Flooding has stopped",
                ["FloodAlStopped"] = "Flooding is already stopped.",
                ["Reset"] = "Ocean level has reset.",
                ["UkCmd"] = "Unknown command, please use /wf help.",
                ["HelpMenu"] = "===== HELP =====\n\n/wf help | Opens help menu\n\n/wf flood <Height> <Time> | Floods The map to a specific height for a duration of time (in seconds)\n\n/wf stop | Stops the flood\n\n/wf reset | Resets the water level\n\n===== HELP =====",
                ["FloodCmd"] = "flood",
                ["StopCmd"] = "stop",
                ["ResetCmd"] = "reset",
                ["HelpCmd"] = "help"
            }, this);
        }



        [Command("wf"), Permission("waterflow.use")]   
        private void floodcmd(IPlayer player, string command, string[] func)
        {
            int initcmd = 0;      

            if (func[0] == lang.GetMessage("HelpCmd", this, player.Id)) initcmd = 0;
            else if (func[0] == lang.GetMessage("FloodCmd", this, player.Id)) initcmd = 1;
            else if (func[0] == lang.GetMessage("StopCmd", this, player.Id)) initcmd = 2;
            else if (func[0] == lang.GetMessage("ResetCmd", this, player.Id)) initcmd = 3;
            else player.Reply(lang.GetMessage("UkCmd", this, player.Id));



            switch (initcmd)
            {
                case 0:   
                    player.Reply(lang.GetMessage("HelpMenu", this, player.Id));
                    break;
                case 1:  
                    double height = int.Parse(func[1]);
                    double seconds = int.Parse(func[2]);

                    double downheight = WaterSystem.OceanLevel - height;           
                    double upheight = height - WaterSystem.OceanLevel;           

                    double newadd;

                    if (WaterSystem.OceanLevel < height)
                    {
                        newadd = upheight / seconds;     

                        player.Reply(lang.GetMessage("FloodStart", this, player.Id));

                        ftimer = timer.Every(1, () =>      
                        {
                            if (WaterSystem.OceanLevel >= height) ftimer.Destroy();
                            else addolevel(newadd);
                        });
                        return;
                    }

                    if(WaterSystem.OceanLevel > height)
                    {
                        player.Reply(lang.GetMessage("FloodDown", this, player.Id));
                        newadd = downheight / seconds;     

                        ftimer = timer.Every(1, () =>      
                        {
                            if (WaterSystem.OceanLevel <= height) ftimer.Destroy();
                            else takeolevel(newadd);
                        });
                        return;
                    }

                    break;
                case 2:   
                    if(ftimer.Destroyed == true)
                    {
                        player.Reply(lang.GetMessage("IsAlStopped", this, player.Id));
                        break;
                    }
                    else
                    {
                        ftimer.Destroy();
                        player.Reply(lang.GetMessage("FloodStop", this, player.Id));
                        break;
                    }

                case 3:   
                    resetolevel();
                    player.Reply(lang.GetMessage("Reset", this, player.Id));
                    break;
            }

        }
        
        private void addolevel(double oceanlevel)
        {
            server.Command("meta.add", "oceanlevel", oceanlevel.ToString());
        }
        private void takeolevel(double oceanlevel)
        {
            oceanlevel = oceanlevel * -1;
            server.Command("meta.add", "oceanlevel", oceanlevel.ToString());
        }
        private void setolevel(double oceanlevel)
        {
            server.Command("oceanlevel", oceanlevel.ToString());
        }
        private void resetolevel()
        {
            server.Command("oceanlevel", "0");
        }
    }
}
