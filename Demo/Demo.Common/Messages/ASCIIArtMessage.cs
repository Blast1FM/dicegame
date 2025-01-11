using System;
using DiceGame.Common.Messages;

namespace Demo.Common.Messages;

public class ASCIIArtMessage : BaseMessage
{
    public string? Name { get; set; }
    public string? Art { get; set; }

    public ASCIIArtMessage(string name, string art)
    {
        Name = name;
        Art = art;
    }
}

public static class ASCIIArt
{
    public static string GetArtString()
    {
        return @"   ⠄⡄⡆⡄⠄⠄⠄⠄⠄⢀⡤⠄⠒⠄⠄⠄⣄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⣄⣠⠄
                    ⢱⣼⣿⠇⢀⠄⠄⠄⡰⠅⢀⣴⣾⡿⠿⢷⠝⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⣿⣿⡝
                    ⠸⣿⣿⠗⠋⠄⠄⠠⠂⠄⣿⣿⡧⢒⣂⣼⣿⣄⡚⡄⠄⠄⠄⠄⠄⠈⠲⣿⣿⡯
                    ⠄⣿⡿⠄⠄⠄⠄⠄⠄⠄⣿⣿⣿⣿⡏⣩⣤⣵⡠⡺⡄⠄⠄⠄⠄⠄⠄⢸⣿⠃
                    ⠄⣿⣿⠄⠄⠄⠄⠄⢀⢀⡘⣿⣿⣿⣧⠋⠖⠚⠂⢹⣿⠄⠄⠄⠄⠄⠄⣾⣿⠄
                    ⠄⣿⣷⡇⠄⠄⠄⠄⢁⣏⣷⣿⣿⣿⣿⣿⣿⣟⡲⠾⢻⠄⠄⠄⠄⠄⣸⣿⡟⠄
                    ⠄⢹⣿⣿⡀⠄⠄⠄⠄⠉⢫⣼⣿⣿⣿⣿⣿⠟⠓⠓⡘⠄⠄⠄⠄⣰⣿⣿⠁⠄
                    ⠄⠈⠿⠿⠿⠄⠄⠄⠄⠄⠄⠻⠿⠿⠿⠟⠉⠄⠄⠹⠇⠄⠄⠄⠺⠿⠿⠏⠄⠄";
    }
}