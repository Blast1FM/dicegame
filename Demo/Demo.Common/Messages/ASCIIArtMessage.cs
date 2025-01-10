using System;
using DiceGame.Common.Messages;

namespace Demo.Common.Messages;

public class ASCIIArtMessage(string name, string art) : BaseMessage
{
    public string Name { get; set; } = name;
    public string Art { get; set; } = art;
}
