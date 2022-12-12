using Telegram.Bot;

class interpreter{
    public Dictionary<string,int>context;
    lexer lex;
    token current_token;
    string code;
    ITelegramBotClient botClient;
    List<int>chat_ids;
    public interpreter(ITelegramBotClient botClient,string s,Dictionary<string,int>cont,List<int>chat_ids){
        this.botClient=botClient;
        this.chat_ids=chat_ids;
        code=s;
        context=new Dictionary<string, int>();

        List<string>vars=new List<string>();
        foreach(var item in cont){
            context.Add(item.Key,item.Value);
            vars.Add(item.Key);
        }
        lex=new lexer(code,vars);
    
        current_token=lex.get_next_token();
    }

    public void notify_members(string message){
        foreach(int chat_id in chat_ids){
            tlg.send_message(botClient,chat_id,message);
        }
    }

    public void error(){
        throw new Exception("Invalid syntax");
    }
    public void eat(string token_type){
        if(current_token.type==token_type)
            current_token=lex.get_next_token();
        else
            error();
    }
    public int factor(){
        token token = current_token;
        
        if(token.type == "INTEGER"){
            eat("INTEGER");
            return int.Parse(token.value);
        }
        if(token.type == "VAR"){
            string vname=token.value;
            eat("VAR");
            return context[vname];
        }
        if(token.type == "LPAREN"){
            eat("LPAREN");
            int result = expr();
            eat("RPAREN");
            return result;
        }
        return 0;
    }
    public int term(){
        int result = factor();
        while(current_token.type=="MUL" || current_token.type=="DIV"){
            token token = current_token;
            if(token.type=="MUL"){
                eat("MUL");
                result*=factor();
            }
            else if(token.type=="DIV"){
                eat("DIV");
                result/=factor();
            }
        }
        return result;
    }
    public int expr(){
        int result = term();
        while(current_token.type=="PLUS" || current_token.type=="MINUS"){
            token token = current_token;
            if(token.type=="PLUS"){
                eat("PLUS");
                result+=term();
            }
            else if(token.type=="MINUS"){
                eat("MINUS");
                result-=term();
            }
        }
        return result;
    }


    public string str_term(){
        string result="";
        if(current_token.type=="STRING"){
            result=current_token.value;
            eat("STRING");
        }
        if(current_token.type=="INTEGER"){    
           result=current_token.value.ToString();
           eat("INTEGER");
        }
        
        if(current_token.type=="VAR"){
           result+=context[current_token.value].ToString();
           eat("VAR");
        }
        return result;
    }
    
    public string str_expr(){
        string result=str_term();
        while(current_token.type!="SCOL" && current_token.type!= "RPAREN"){
            token token = current_token;
            if(token.type=="PLUS"){
                eat("PLUS");
                result+=str_term();
            }
        }
        Console.WriteLine(result);
        return result;
    }

    public void line(){
        token token = current_token;
        Console.WriteLine(current_token.type+" "+current_token.value);
        
        if(token.type=="NOTI"){//Notification
            eat("NOTI");
            Console.WriteLine(current_token.type+" "+current_token.value);
            eat("LPAREN");
            Console.WriteLine(current_token.type+" "+current_token.value);
            notify_members(str_expr());
            Console.WriteLine(current_token.type+" "+current_token.value);
            eat("RPAREN");
            Console.WriteLine(current_token.type+" "+current_token.value);
            eat("SCOL");
        }      
        if(token.type=="INT"){//Integer declaration
            eat("INT");
            Console.WriteLine(current_token.type+" "+current_token.value);
            string vname=current_token.value;
            eat("VAR");
            Console.WriteLine(current_token.type+" "+current_token.value);
            eat("ASG");
            Console.WriteLine(current_token.type+" "+current_token.value);
            int value=expr();
            if(!context.ContainsKey(vname))
                context.Add(vname,value);
            else
                context[vname]=value;
            Console.WriteLine(current_token.type+" "+current_token.value);
            eat("SCOL");
        }
        if(token.type=="VAR"){//Integrer modification
            string vname=current_token.value;
            eat("VAR");
            Console.WriteLine(current_token.type+" "+current_token.value);
            eat("ASG");
            Console.WriteLine(current_token.type+" "+current_token.value);
            int value=expr();
            if(!context.ContainsKey(vname))
                context.Add(vname,value);
            else
                context[vname]=value;
            Console.WriteLine(current_token.type+" "+current_token.value);
            eat("SCOL");
        }       
    }

    public void run(){
        while(current_token.type!="}" && current_token.type!="EOF" ){
            token token = current_token;
            if(token.type=="IF"){
                eat("IF");
                continue;
            }
            if(token.type=="WHILE"){
                eat("WHILE");
                continue;
            }
            line();
        }
    }

}

