using UnityEngine;

public class MyLogger
{
    private static MyLogger instance;

    private Logger logger;

    public static MyLogger Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new MyLogger();
            }
            return instance;
        }
    }

    private MyLogger()
    {
        logger = new Logger(new MyLogHandler());
    }
}
