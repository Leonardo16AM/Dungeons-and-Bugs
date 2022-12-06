using Telegram.Bot;

class interpreter{
    Dictionary<string,int>context;
    lexer lex;
    token current_token;
    string code;
    ITelegramBotClient botClient;

    public interpreter(ITelegramBotClient botClient,string s,Dictionary<string,int>cont){
        this.botClient=botClient;
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

    public void line(){
        token token = current_token;
        Console.WriteLine(current_token.type+" "+current_token.value);
        if(token.type=="NOTI"){
            eat("NOTI");
            Console.WriteLine(current_token.type+" "+current_token.value);
            eat("LPAREN");
            Console.WriteLine(current_token.type+" "+current_token.value);
            eat("STRING");
            Console.WriteLine(current_token.type+" "+current_token.value);
            eat("RPAREN");
            Console.WriteLine(current_token.type+" "+current_token.value);
            eat("SCOL");
        }      
        if(token.type=="STR"){
            eat("STR");
            Console.WriteLine(current_token.type+" "+current_token.value);
            eat("VAR");
            Console.WriteLine(current_token.type+" "+current_token.value);
            eat("EQ");
            Console.WriteLine(current_token.type+" "+current_token.value);
            eat("STRING");
            Console.WriteLine(current_token.type+" "+current_token.value);
            eat("SCOL");
        }          
        
        if(token.type=="INT"){
            eat("INT");
            Console.WriteLine(current_token.type+" "+current_token.value);
            eat("VAR");
            Console.WriteLine(current_token.type+" "+current_token.value);
            eat("EQ");
            Console.WriteLine(current_token.type+" "+current_token.value);
            eat("INTEGER");
            Console.WriteLine(current_token.type+" "+current_token.value);
            eat("SCOL");
        }       
    }

    public void cacho(){
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

