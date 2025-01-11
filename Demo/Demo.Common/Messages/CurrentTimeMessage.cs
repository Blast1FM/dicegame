using System;
using DiceGame.Common.Messages;

namespace Demo.Common.Messages;

public class CurrentTimeMessage : BaseMessage
{
    public DateTime? CurrentTime { get; set; }
    public CurrentTimeMessage(DateTime currentTime)
    {   
        CurrentTime = currentTime;
    }
    public CurrentTimeMessage(){}
}
