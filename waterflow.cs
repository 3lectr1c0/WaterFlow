using System.Collections.Generic;
using Oxide.Core.Libraries.Covalence;

namespace Oxide.Plugins
{
    [Info("WaterFlow", "3LECTR1C", "0.1.0")]
    [Description("Various water related commands.")]
    public class waterflow : CovalencePlugin
    {
        private Timer ftimer;

        protected override void LoadDefaultMessages()
        {
            lang.RegisterMessages(new Dictionary<string, string>
            {
                ["FloodStart"] = "The map is now flooding.",
                ["FloodStop"] = "Flooding has stopped",
                ["FloodAllStopped"] = "Flooding is already stopped.",
                ["Reset"] = "Ocean level has reset.",
                ["UnkCmd"] = "Unknown command, please use /wf help.",
                ["HelpMenu"] = "===== HELP =====\n\n/wf help | Opens help menu\n\n/wf flood <Height> <Time> | Floods The map to a specific height for a duration of time (in seconds)\n\n/wf stop || Stops the flood\n\n/wf reset | Resets the water level\n\n===== HELP ====="
            }, this);
        }


        [Command("wf"), Permission("waterflow.use")]
        private void floodcmd(IPlayer player, string command, string[] func)
        {
            int initcmd = 0; // int for command to run

            // defines command number
            if (func[0] == "help") initcmd = 0;
            else if (func[0] == "flood") initcmd = 1;
            else if (func[0] == "stop") initcmd = 1;
            else if (func[0] == "reset") initcmd = 3;



            // switches based on command
            switch (initcmd)
            {
                case 0: // help command
                    player.Reply(lang.GetMessage("HelpMenu", this, player.Id));
                    break;
                case 1:// flood command
                    player.Reply(lang.GetMessage("FloodStart", this, player.Id));
                    double height = int.Parse(func[1]);
                    double seconds = int.Parse(func[2]);

                    double downheight = WaterSystem.OceanLevel - height; // if height was 10 and oceanlevel was 50 = 40
                    double upheight = height - WaterSystem.OceanLevel; // if height was 50 and oceanlevel was 10 = 40

                    double newadd;

                    if (WaterSystem.OceanLevel < height)
                    {
                        newadd = upheight / seconds; // sets rate for time

                        // Up
                        ftimer = timer.Every(1, () =>
                        {
                            player.Reply("up");
                            if (WaterSystem.OceanLevel >= height) ftimer.Destroy();
                            else addolevel(newadd);
                        });
                        return;
                    }

                    if(WaterSystem.OceanLevel > height)
                    {
                        // down
                        player.Reply("down");
                        newadd = downheight / seconds; // sets rate for time

                        ftimer = timer.Every(1, () =>
                        {
                            if (WaterSystem.OceanLevel <= height) ftimer.Destroy();
                            else takeolevel(newadd);
                        });
                        return;
                    }

                    break;
                case 2: // stop command
                    ftimer.Destroy();
                    break;
                case 3: // reset command
                    resetolevel();
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
