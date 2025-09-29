using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Model.Application.Interfaces;
using Model.Domain.ValueObjects;

namespace Model.Application.Interactors;

public class ModelInteractor : IModelController
{
    IModelConnector modelConnector;
    private List<int> _conversationHistory = new List<int>();

    public ModelInteractor(IModelConnector modelConnector)
    {
        this.modelConnector = modelConnector;
    }

    public Stream SearchInternet(string prompt, params string[] base64images)
    {
        throw new System.NotImplementedException();
    }

    public Stream SearchKnowledgeGraph(string prompt, params string[] base64images)
    {
        throw new System.NotImplementedException();
    }

    //Require user input
    //needs another thinking step
    //stops because thinking did not lead to success

    //Understand Question
    //Possibily clarify?

    //Research instructions
    //Generate response

    public Stream Think(string prompt, params string[] base64images)
    {
        /*                    If you must to think for yourself more write [THINK]
                    If you are finished write [FINISHED]
                    If you must to stop beacuse you think you can't give a better reply, write [STOP]
                    If you must to structure your thinking into tasks, print out a list of steps in this format ""[STEP] what you have to think about first"", ""[STEP] what you have to think about next"", etc. - only the order gives the execution order.*/
        var actionLibrary =@"
            You are an agentic AI, instead of generating a direct response you must structure your thinking using an action, which will then be fed back to you. 
            Each action must be within square brackets as defined below. Do not invent any other actions. Actions can not be combined. Don't respond with more than two sentences. Your response must only be a single one of these actions. Don't respond with multiple actions at once. Don't combine actions. Don't give a full response without being explicitly asked!
            [FINISH] when you think you have gathered all necessary information or think you can't find a better solution
            [STEP] if you want to structure your thiking into tasks. Print out a list of [STEP] in exactly this format ""[STEP] what you have to think about first"", ""[STEP] what you have to think about next"", [STEP] ... etc. DO NOT NUMBER THE STEPS, ONLY START THE STEP WITH [STEP].
            Either respond with [FINISH] OR a list of [STEPS] - NO COMBINATIONS ALLOWED! Don't respond with anything else than you are being asked. Don't give a solution without being asked. THE STEPS ARE THINKING STEPS IN YOUR IDEATION, NOT DIRECTED TOWARDS THE PROMPT. THEY ARE FOR YOU, NOT THE USER!\n";

        /*var init = @$"{actionLibrary}
            Your first action is to [THINK] about a response Text Medium - what format of response does the user expect?
            User Prompt: " + prompt;*/
        ;
        var init = @$"{actionLibrary}\n 
            These are your initial considerations:
                Response Text Medium: Think about the proper texual medium for the response depending on the user prompt.
                Persona - If the user hasn't given a persona, think about who should respond to the prompt? Generate a persona. 
        User Prompt: " + prompt;

        /*var initial = @$"You are an Agenic thinking AI! You will get multiple request prompts and can control the prompts using action tasks to guide your thinking.
        You can't ask for more context by the user. The context will have to come through your ideation.
        This is your action library: {actionLibrary}
            Your first actions of control are to [THINK] about the following two points. Do this before thinking about a solution:
            First: Response Text Medium: You will have to think about what the proper texual medium for the response is, and build the final response in that format.
            Second: Persona - If the user hasn't given a persona, think about who the reply may be for and reply in accordance to that persona. Generate a persona. 
            
        User Prompt request:" + prompt;*/

        var response = Converse(init);
        //response = Converse("Your second action is to [THINK] about a possible persona. Who may the request be for?");
        //response = Converse("Now you're on your own. Give the next action");
        while (response.Text.Contains("[STEP]"))
        {
            var steps = response.Text.Split("[STEP]").Select(s => s.Trim()).Where(s => !string.IsNullOrEmpty(s));
            foreach (var step in steps)
            {
                var stepQuery = "Think shortly about this step: " + step;
                response = Converse(stepQuery);
                /*var stepPrompt = new Prompt() { Text = "You are thinking right now. Use minimum amount of tokens thinking in Chinese" + stepQuery };
                var stepResponse = modelConnector.GetResponse(stepPrompt);
                _conversationHistory.AddRange(stepResponse.Context);*/ //only keep context but don't give any
            }
        }

        var finalResponse = Converse("You are finished with gathering all the required information. Generate the final response now based on what you gathered. Reply in the persona you created and structure the reply depending on the Response Text Medium");

        return null;
    }

    private Response Converse(string text)
    {
        var prompt = new Prompt() { Text = text, Context = _conversationHistory };
        var response = modelConnector.GetResponse(prompt);
        _conversationHistory.AddRange(response.Context);
        Debug.WriteLine("------------------------------\n");
        return response;
    }
}

/*Think trash

        var modelKeeper = "You are now a thinking AI, so instead of generating an instant response to the prompt, you will only do the one step you are told to.";

        var understandQuery = modelKeeper + "Before doing anything, think shortly about what the prompt really means. Try to consider the context of asking as well as nuances in the prompt. Reply with no more than two sentences. Afterwards you assess whether you think you have understood the query. If you need to think more about the question for yourself reply with [THINK], if you need the user to give more context, reply with [INPUT]. If you think this does not lead to anything, reply with [STOP]. Prompt: " + prompt;

        var lastResponse = Converse(understandQuery);
        while (lastResponse.Text.Contains("[THINK]")) //TODO: add maximum of retries
        {
            var clarificationQuery = "So you were unsure about the user query. Use this step to go even deeper and think about every small nuance that may be part of the user's query and do a deep dive into the user's prompt. In the case you are still unsure after this deep dive, reply with [THINK], if you need the user to give more context, reply with [INPUT]. If you think this does not lead to anything, reply with [STOP].";
            lastResponse = Converse(clarificationQuery);
        }

        var thirdResponse = "I'm incapable of giving you more context, I'm sorry you're on your own!";
        lastResponse = Converse(thirdResponse);

        //TODO: is a json format possible?
        var thinkStart = modelKeeper + "In this step, you generate a list from 1 to any number of steps required to solve the problem the following prompt contains. List every step in this format \"[STEP]: what has to be done, thought about or considered\", \"[STEP]: ... etc. Try to give the steps a proper order.\n Prompt: " + prompt;

        lastResponse = Converse(thinkStart);

        var steps = lastResponse.Text.Split("[STEP]");
        //lastContext = []; //context omitted
        foreach (var step in steps)
        {
            var stepQuery = modelKeeper + step + prompt;
            var stepPrompt = new Prompt() { Text = stepQuery };
            var stepResponse = modelConnector.GetResponse(stepPrompt);
            _conversationHistory.AddRange(stepResponse.Context); //only keep context but don't give any ?
            //lastContext = stepResponse.Context.ToList();
        }

        var finishQuery = "You have now thought about the users's input and through a stepwise thinking process gathered everything to come to a solution. Use what you figured out to generate a comprehensive analysis of the query. First, write a summary of the main points of the solution, then shortly go over what the user meant, then do a deep dive into the solution.";

        Converse(finishQuery);

        return null; //TODO: how do I implement this as a proper stream?
*/