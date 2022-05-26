namespace VirtualMeetingApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (!File.Exists("people.json"))
            {
                Menu.Output("Please make sure that people.json file exists in the root directory!");
                Menu.Output("Example of people.json entry:\n[{\n\t\"name\": \"Mazvydas Janiskevicius\"\n\t\"pin\":\"1234\"\n}]");
                Environment.Exit(0);
            }

            MeetingApp app = new MeetingApp();

            app.RunApp();            
        }
    }
}