using System;
using System.Text.Json.Serialization;
using DiceGame.Common.Messages;

namespace Demo.Common.Messages;

public class CurrentTimeMessage : BaseMessage
{
    public Guid? SessionToken {get;set;}
    public DateTime CurrentTime { get; set; }
    public CurrentTimeMessage(Guid sessionToken, DateTime currentTime)
    {   
        SessionToken = sessionToken;
        CurrentTime = currentTime;
    }
    public CurrentTimeMessage(){}
}
