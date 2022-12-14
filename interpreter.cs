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
        Console.WriteLine(">>>>>> "+current_token.type);
        if(current_token.type==token_type)
            current_token=lex.get_next_token();
        else
            error();
    }
    public int int_factor(){
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
            int result = int_expr();
            eat("RPAREN");
            return result;
        }
        return 0;
    }


    public int int_term(){
        int result = int_factor();
        while(current_token.type=="MUL" || current_token.type=="DIV"){
            token token = current_token;
            if(token.type=="MUL"){
                eat("MUL");
                result*=int_factor();
            }
            else if(token.type=="DIV"){
                eat("DIV");
                result/=int_factor();
            }
        }
        return result;
    }
    public int int_expr(){
        int result = int_term();
        while(current_token.type=="PLUS" || current_token.type=="MINUS"){
            token token = current_token;
            if(token.type=="PLUS"){
                eat("PLUS");
                result+=int_term();
            }
            else if(token.type=="MINUS"){
                eat("MINUS");
                result-=int_term();
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

    bool is_bool(){
        token wr=current_token;
        bool ret=false;
        string last_token=lex.last_token;
        int pos=lex.pos;
        char current_char=lex.current_char;
        int cnt=1;
        eat("LPAREN");
        while(cnt!=0){
            token token=current_token;
            if(token.type=="LPAREN")cnt++;
            if(token.type=="RPAREN")cnt--;
            if(token.type=="EQ"||token.type=="DIF"||token.type=="LET"||token.type=="GET"||token.type=="GT"||token.type=="LT")ret=true;        
            eat(token.type);
        }
        current_token=wr;
        lex.last_token=last_token;
        lex.pos=pos;
        lex.current_char=current_char;
        return ret;
    }

    bool bool_term(){
        if(current_token.type=="LPAREN" && is_bool() ){
            Console.WriteLine("is bool");
            eat("LPAREN");
            bool ret=bool_expr();
            eat("RPAREN");
            return ret;
        }

        int a=int_factor();
        string comp_type=current_token.type;
        eat(comp_type);
        int b=int_factor();
        Console.WriteLine(a);
        Console.WriteLine(comp_type);
        Console.WriteLine(b);
        if(comp_type=="EQ"){return a==b;}
        if(comp_type=="DIF"){return a!=b;}
        if(comp_type=="GT"){return a>b;}
        if(comp_type=="LT"){return a<b;}
        if(comp_type=="GET"){return a>=b;}
        if(comp_type=="LET"){return a<=b;}
        return false;
    }

    public bool bool_expr(){
        bool ret=bool_term();
        while(current_token.type!="SCOL" && current_token.type!= "RPAREN"){
            token token = current_token;
            if(token.type=="AND"){
                eat("AND");
                ret=(ret&bool_term());
                continue;
            }
            if(token.type=="OR"){
                eat("OR");
                ret=(ret|bool_term());
                continue;
            }
        }
        Console.WriteLine(ret);
        return ret;
    }

    public void line(){
        token token = current_token;
        
        if(token.type=="NOTI"){//Notification
            eat("NOTI");
            eat("LPAREN");
            notify_members(str_expr());
            eat("RPAREN");
            eat("SCOL");
        }      
        if(token.type=="INT"){//Integer declaration
            eat("INT");
            string vname=current_token.value;
            eat("VAR");
            eat("ASG");
            int value=int_expr();
            if(!context.ContainsKey(vname))
                context.Add(vname,value);
            else
                context[vname]=value;
            eat("SCOL");
        }
        if(token.type=="VAR"){//Integrer modification
            string vname=current_token.value;
            eat("VAR");
            eat("ASG");
            int value=int_expr();
            if(!context.ContainsKey(vname))
                context.Add(vname,value);
            else
                context[vname]=value;
            eat("SCOL");
        }     
    }

    void pass(){
        int cnt=1;
        while(cnt!=0){
            string type=current_token.type;
            if(type=="LKEY"){cnt++;}
            if(type=="RKEY"){cnt--;}
            eat(type);
        }
    }
    void block(){
        while(current_token.type!="RKEY" && current_token.type!="EOF" ){
            token token = current_token;
            
            if(token.type=="IF"){
                eat("IF");
                eat("LPAREN");
                bool bex=bool_expr();
                eat("RPAREN");
                eat("LKEY");
                if(bex){
                    block();
                    eat("RKEY");
                }else{
                    pass();
                }
                if(current_token.type=="ELSE"){
                    eat("ELSE");
                    eat("LKEY");
                    if(!bex){
                        block();
                        eat("RKEY");
                    }else{
                        pass();
                    }
                }
                continue;
            }

            line();
        }
    }

    public void run(){
        block();
    }

}

