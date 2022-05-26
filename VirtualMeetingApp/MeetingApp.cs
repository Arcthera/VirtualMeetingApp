using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualMeetingApp
{
    internal class MeetingApp
    {
        readonly Person currentPerson;

        public MeetingApp()
        {
            currentPerson = LoginUser();            
        }

        public void RunApp()
        {
            int selectedOption = 0;

            Menu.Output($"Welcome back, {currentPerson.Name}");

            do
            {
                Menu.DisplayOptions();

                try
                {
                    selectedOption = int.Parse(Menu.ReadUserInput());

                    switch (selectedOption)
                    {
                        case 1:
                            AddMeeting();
                            break;
                        case 2:
                            DeleteMeeting();
                            break;
                        case 3:
                            AddPersonToMeeting();
                            break;
                        case 4:
                            RemovePersonFromMeeting();
                            break;
                        case 5:
                            ListAllMeetings();
                            break;
                        default:
                            Menu.DisplayOptions();
                            break;
                    }
                }
                catch (FormatException)
                {
                    Menu.Output("\nPlease enter a valid option!");
                    Menu.WaitForInput();
                }

            } while (selectedOption != 6);

            Menu.Output("\nSee you soon!");
        }

        private static Person LoginUser()
        {
            string name, pin;
            Person? person;

            Menu.Intro();

            do
            {
                name = Menu.ReadUserInput("Please enter your full name: ");
                person = Database.GetPerson(name);

                if (person == null)
                {
                    Menu.Output($"Person {name} was not found! Please try again.");
                }

            } while (person == null);

            do
            {
                pin = Menu.ReadUserInput("Please enter your pin: ");

                if(pin != person.Pin)
                {

                    Menu.Output($"The pin is incorrect! Please try again.");
                }

            } while (pin != person.Pin);

            return person;
        }

        private void AddMeeting()
        {
            string name, description;
            Person responsiblePerson;
            MeetingCategory category;
            MeetingType type;
            DateTime startDate = new DateTime(), endDate = new DateTime();

            bool validCategory = false, validType = false, validStartDate = false, validEndDate = false;

            name = Menu.ReadUserInput("Please enter a name for this meeting: ");
            description = Menu.ReadUserInput("Please enter a description of this meeting: ");

            responsiblePerson = SelectPerson("Please select the person responsible for this meeting: ");


            do
            {
                Menu.Output("Please enter one of the categories listed below: ");
                
                foreach (MeetingCategory categoryName in Enum.GetValues(typeof(MeetingCategory)))
                {
                    Menu.Output($"\t> {categoryName}");
                }

                if(!Enum.TryParse(Menu.ReadUserInput(), out category))
                {
                    Menu.Output("Please enter a valid option..!");
                    Menu.WaitForInput();
                    Menu.Clear();
                }
                else
                {
                    validCategory = true;
                }
            } while (!validCategory);

            do
            {
                Menu.Output("Please enter one of the types listed below: ");

                foreach (MeetingType typeName in Enum.GetValues(typeof(MeetingType)))
                {
                    Menu.Output($"\t> {typeName}");
                }

                if (!Enum.TryParse(Menu.ReadUserInput(), out type))
                {
                    Menu.Output("Please enter a valid option..!");
                    Menu.WaitForInput();
                    Menu.Clear();
                }
                else
                {
                    validType = true;
                }
            } while (!validType);

            do
            {
                string date = Menu.ReadUserInput("Please enter start date of the meeting (dd/mm/yyyy hh:mm)");

                try
                {
                    startDate = DateTime.ParseExact(date.Trim(), "dd/MM/yyyy HH:mm", null);
                    validStartDate = true;
                }
                catch (FormatException)
                {
                    Menu.Output("The date entered is not in the correct format..!");
                    Menu.WaitForInput();
                    Menu.Clear();
                }
            } while (!validStartDate);

            do
            {
                string date = Menu.ReadUserInput("Please enter end date of the meeting (dd/mm/yyyy hh:mm)");

                try
                {
                    endDate = DateTime.ParseExact(date.Trim(), "dd/MM/yyyy HH:mm", null);
                    validEndDate = true;
                }
                catch (FormatException)
                {
                    Menu.Output("The date entered is not in a correct format..!");
                    Menu.WaitForInput();
                    Menu.Clear();
                }
            } while (!validEndDate);

            Meeting newMeeting = new Meeting(name, responsiblePerson.Name, description, category, type, startDate, endDate);
            newMeeting.AddPersonToMeeting(responsiblePerson);

            Database.AddNewMeeting(newMeeting);
        }

        private void DeleteMeeting()
        {
            List<Meeting> responsibleForMeetings = Database.GetResponsibleForMeetings(currentPerson);

            if(responsibleForMeetings.Count < 1)
            {
                Menu.Output("You are not responsible for any meetings.");
                Menu.Output("Only people resonsable for a certain meeting can delete it!");
                Menu.WaitForInput();
            }
            else
            {
                bool validSelection = false;
                do
                {
                    Menu.Clear();
                    Menu.Output("Please select which meeting you would like to delete: ");
                    for (int i = 0; i < responsibleForMeetings.Count; i++)
                    {
                        Meeting meeting = responsibleForMeetings[i];
                        Menu.Output($"{i}.\n\t{meeting.Name}\n\tDescription: {meeting.Description}\n\tStart date: {meeting.StartDate}\n\tEnd date: {meeting.EndDate}");
                    }

                    if (int.TryParse(Menu.ReadUserInput(), out int selection) && IsSelectionInRange(0, responsibleForMeetings.Count, selection))
                    {
                        Database.DeleteMeeting(responsibleForMeetings[selection]);
                        Menu.Output("Selected meeting was deleted successfully!");
                        Menu.WaitForInput();
                        validSelection = true;
                    }
                    else
                    {
                        Menu.Output("Please select a valid meeting!");
                        Menu.WaitForInput();
                        Menu.Clear();
                    }
                } while (!validSelection);
            }
        }

        private void AddPersonToMeeting()
        {
            Meeting selectedMeeting;
            Person selectedPerson;

            selectedMeeting = SelectMeeting();
            selectedPerson = SelectPerson();

            if (MeetingOverlaps(selectedPerson, selectedMeeting))
            {
                string response = Menu.ReadUserInput("This meeting overlaps with another meeting this person is attending, are you sure you want to continue? (type \"no\" to abort)");

                if(response == "no") return;
            }

            if (selectedMeeting.PersonAlreadyExistsInMeeting(selectedPerson))
            {
                Menu.Output("The selected person already exists in this meeting!");
                Menu.WaitForInput();
                return;
            }
           
            selectedMeeting.AddPersonToMeeting(selectedPerson);
        }

        private void RemovePersonFromMeeting()
        {
            Meeting selectedMeeting;
            Person selectedPerson;

            selectedMeeting = SelectMeeting();
            selectedPerson = SelectPerson();

            if (!selectedMeeting.PersonAlreadyExistsInMeeting(selectedPerson))
            {
                Menu.Output("The selected person is not attending this meeting!");
                Menu.WaitForInput();
                return;
            }

            if (selectedMeeting.IsResponsablePerson(selectedPerson)){
                Menu.Output("The person responsible for a meeting cannot be removed!");
                Menu.WaitForInput();
                return;
            }

            selectedMeeting.RemovePersonFromMeeting(selectedPerson);
        }

        private void ListAllMeetings()
        {
            Menu.Output("Enter any parameters to filter the list (optional)");
            Menu.Output("Available parameters:");
            Menu.Output("\t-d .NET (filters by description)");
            Menu.Output("\t-rp John Doe (filters by responsable person)");
            Menu.Output("\t-c Hub (filters by category)");
            Menu.Output("\t-t Live (filters by type)");
            Menu.Output("\t-sd dd/mm/yyyy (filters starting after a certain date)");
            Menu.Output("\t-ed dd/mm/yyyy (filters ending before a certain date)");
            Menu.Output("\t-an <5 (filters by number of atendees, <X - less than X, > - more than X, =X equal to X)");

            string value = Menu.ReadUserInput().Trim();

            Menu.Output("\nMeetings found with the specified filters: ");

            if (String.IsNullOrEmpty(value))
            {
                for (int i = 0; i < Database.Meetings.Count; i++)
                {
                    Meeting meeting = Database.Meetings[i];

                    Menu.Output("-----------------------------------------");
                    PrintMeetingDetails(meeting);
                    Menu.Output("-----------------------------------------");
                }
            }
            else
            {
                string[] parameters = value.Split("-");
                List<Meeting> filteredList = new List<Meeting>(Database.Meetings);

                for (int i = 0; i < parameters.Length; i++)
                {
                    string parameter = parameters[i];

                    if (String.IsNullOrEmpty(parameter)) continue;

                    try
                    {
                        string filter = parameter.Split()[0].Trim();
                        string filterValue = parameter.Split()[1].Trim();

                        switch (filter)
                        {
                            case "d":
                                filteredList = filteredList.Where(m => m.Description.Contains(filterValue)).ToList();
                                break;
                            case "rp":
                                filteredList = filteredList.Where(m => m.ResponsiblePerson.Equals(filterValue)).ToList();
                                break;
                            case "c":
                                if(Enum.IsDefined(typeof(MeetingCategory), filterValue))
                                {
                                    filteredList = filteredList.Where(m => Enum.GetName(typeof(MeetingCategory), m.Category).Equals(filterValue)).ToList();
                                }
                                else
                                {
                                    Menu.Output($"Unknown category {filterValue}, skipping...");
                                    continue;
                                }
                                
                                break;
                            case "t":
                                if (Enum.IsDefined(typeof(MeetingType), filterValue))
                                {
                                    filteredList = filteredList.Where(m => Enum.GetName(typeof(MeetingType), m.Type).Equals(filterValue)).ToList();
                                }
                                else
                                {
                                    Menu.Output($"Unknown type {filterValue}, skipping...");
                                    continue;
                                }
                                break;
                            case "sd":
                                try
                                {
                                    DateTime startDate = DateTime.ParseExact(filterValue.Trim(), "dd/MM/yyyy", null);

                                    filteredList = filteredList.Where(m => DateTime.Compare(m.StartDate, startDate) > 0).ToList();
                                }
                                catch (FormatException)
                                {
                                    Menu.Output("The date entered is not in the correct format, skipping...");
                                    continue;
                                }
                                break;
                            case "ed":
                                try
                                {
                                    DateTime endDate = DateTime.ParseExact(filterValue.Trim(), "dd/MM/yyyy", null);

                                    filteredList = filteredList.Where(m => DateTime.Compare(m.EndDate, endDate) < 0).ToList();
                                }
                                catch (FormatException)
                                {
                                    Menu.Output("The date entered is not in the correct format, skipping...");
                                    continue;
                                }
                                break;
                            case "an":
                                string comparisonOperator = filterValue[..1];

                                if (comparisonOperator.Equals("<") || comparisonOperator.Equals("=") || comparisonOperator.Equals(">"))
                                {
                                    if (!int.TryParse(filterValue[1..], out int number))
                                    {
                                        Menu.Output("Atendee number must be and integer, skipping...");
                                        continue;
                                    }

                                    if (comparisonOperator.Equals("<"))
                                    {
                                        filteredList = filteredList.Where(m => m.Atendees.Count < number).ToList();
                                    }
                                    else if (comparisonOperator.Equals(">"))
                                    {
                                        filteredList = filteredList.Where(m => m.Atendees.Count > number).ToList();
                                    }
                                    else
                                    {
                                        filteredList = filteredList.Where(m => m.Atendees.Count == number).ToList();
                                    }
                                }
                                else
                                {
                                    Menu.Output("Unknown comparison opperator, skipping...");
                                    continue;
                                }

                                break;
                            default:
                                Menu.Output($"Unknown parameter {parameter}, skipping...");
                                break;
                        }
                    }
                    catch (Exception)
                    {
                        continue;
                    }


            }
                for (int i = 0; i < filteredList.Count; i++)
                {
                    Meeting meeting = filteredList[i];

                    Menu.Output("-----------------------------------------");
                    PrintMeetingDetails(meeting);
                    Menu.Output("-----------------------------------------");
                }
            }

            Menu.WaitForInput();
        }

        private void PrintMeetingDetails(Meeting meeting)
        {
            Menu.Output($"Name: {meeting.Name}");
            Menu.Output($"Responsible person: {meeting.ResponsiblePerson}");
            Menu.Output($"Description: {meeting.Description}");
            Menu.Output($"Catergory: {Enum.GetName(typeof(MeetingCategory), meeting.Category)}");
            Menu.Output($"Type: {Enum.GetName(typeof(MeetingType), meeting.Type)}");
            Menu.Output($"Start date: {meeting.StartDate}");
            Menu.Output($"End date: {meeting.EndDate}");
            Menu.Output($"Atendees: {String.Join(", ", meeting.Atendees)}");
        }

        private Meeting SelectMeeting()
        {
            do
            {
                Menu.Output("Select a meeting: ");

                for (int i = 0; i < Database.Meetings.Count; i++)
                {
                    Meeting meeting = Database.Meetings[i];
                    Menu.Output($"{i}.\n\t{meeting.Name}\n\tDescription: {meeting.Description}\n\tStart date: {meeting.StartDate}\n\tEnd date: {meeting.EndDate}");
                }

                if (int.TryParse(Menu.ReadUserInput(), out int selection) && IsSelectionInRange(0, Database.Meetings.Count, selection))
                {
                    return Database.Meetings[selection];
                }
                else
                {
                    Menu.Output("Please select a valid meeting!");
                    Menu.WaitForInput();
                    Menu.Clear();
                }
            } while (true);
        }

        private Person SelectPerson(string prompt = "")
        {
            do
            {
                if (string.IsNullOrEmpty(prompt))
                {
                    Menu.Output("Select a person: ");
                }
                else
                {
                    Menu.Output(prompt);
                }

                for (int i = 0; i < Database.People.Count; i++)
                {
                    Person person = Database.People[i];
                    Menu.Output($"{i}. {person.Name}");
                }

                if (int.TryParse(Menu.ReadUserInput(), out int selection) && IsSelectionInRange(0, Database.People.Count, selection))
                {
                    return Database.People[selection];
                }
                else
                {
                    Menu.Output("Please select a valid person!");
                    Menu.WaitForInput();
                    Menu.Clear();
                }

            } while (true);
        }

        private bool MeetingOverlaps(Person selectedPerson, Meeting selectedMeeting)
        {
            foreach (Meeting meeting in Database.Meetings)
            {
                if (meeting.Equals(selectedMeeting))
                {
                    continue;
                }

                if (meeting.Atendees.Contains(selectedPerson.Name))
                {
                    if(DateTime.Compare(selectedMeeting.StartDate, meeting.EndDate) < 0 && DateTime.Compare(meeting.StartDate, selectedMeeting.EndDate) < 0)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private bool IsSelectionInRange(int start, int end, int selection)
        {
            if(start <= selection && selection < end)
            {
                return true;
            }

            return false;
        }
    }
}
