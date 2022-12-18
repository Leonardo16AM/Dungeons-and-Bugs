using Telegram.Bot;

class interpreter{
    public Dictionary<string,int>context;
    lexer lex;
    token current_token;
    string code;
    ITelegramBotClient botClient;
    List<int>chat_ids;
    public interpreter(ITelegramBotClient botClient,string code,Dictionary<string,int>context,List<int>chat_ids){
        this.botClient=botClient;
        this.chat_ids=chat_ids;
        this.code=code;
        this.context=new Dictionary<string, int>();
        
        List<string>vars=new List<string>();
        if(context!=null)
            this.context=context;
            vars=new List<string>(context.Keys);
        lex=new lexer(code,vars);
        this.current_token=lex.get_next_token();
    }

    public void error(string e){throw new Exception(e);}

    void eat(string token_type){
        Console.WriteLine(">>> "+current_token.type);
        if(current_token.type==token_type)
            current_token=lex.get_next_token();
        else
            error($"Interpreter Error: Expected {token_type} found {current_token.type}");
    }
    int int_factor(){
        token token = current_token;
        
        if(token.type == "INTEGER"){
            eat("INTEGER");
            return int.Parse(token.value);
        }
        if(token.type == "RND"){
            eat("RND");
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


    int int_term(){
        int result = int_factor();
        while(current_token.type=="MUL" || current_token.type=="DIV"){
            token token = current_token;
            if(token.type=="MUL"){
                eat("MUL");
                result*=int_factor();
            }else if(token.type=="DIV"){
                eat("DIV");
                result/=int_factor();
            }
        }
        return result;
    }
    int int_expr(){
        int result = int_term();
        while(current_token.type=="PLUS" || current_token.type=="MINUS"){
            token token = current_token;
            if(token.type=="PLUS"){
                eat("PLUS");
                result+=int_term();
            }else if(token.type=="MINUS"){
                eat("MINUS");
                result-=int_term();
            }
        }
        return result;
    }


    string str_term(){
        string result="";
        if(current_token.type=="STRING"){
            result=current_token.value;
            eat("STRING");
        }
        if(current_token.type=="INTEGER"){    
            result=current_token.value.ToString();
            eat("INTEGER");
        }
        if(current_token.type=="RND"){    
            result=current_token.value.ToString();
            eat("RND");
        }
        if(current_token.type=="VAR"){
           result+=context[current_token.value].ToString();
           eat("VAR");
        }
        return result;
    }
    
    string str_expr(){
        string result=str_term();
        while(current_token.type!="COMA" && current_token.type!="SCOL" && current_token.type!= "RPAREN"){
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
        if(comp_type=="EQ"){return a==b;}
        if(comp_type=="DIF"){return a!=b;}
        if(comp_type=="GT"){return a>b;}
        if(comp_type=="LT"){return a<b;}
        if(comp_type=="GET"){return a>=b;}
        if(comp_type=="LET"){return a<=b;}
        return false;
    }

    bool bool_expr(){
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
        return ret;
    }

    void line(){
        token token = current_token;
        
        if(token.type=="NOTI"){//Notification
            eat("NOTI");
            eat("LPAREN");
            tlg.notify_members(botClient,chat_ids,str_expr());
            eat("RPAREN");
            eat("SCOL");
        }      
        if(token.type=="NOTIP"){//Notification with picture
            eat("NOTIP");
            eat("LPAREN");
            string url=str_expr();
            eat("COMA");
            tlg.notify_members_with_picture(botClient,chat_ids,str_expr(),url);
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
            if(type=="EOF"){return;}
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
                }else pass();
                if(current_token.type=="ELSE"){
                    eat("ELSE");
                    eat("LKEY");
                    if(!bex){
                        block();
                        eat("RKEY");
                    }else pass();
                }
                continue;
            }
            
            if(token.type=="WHILE"){
                eat("WHILE");
                eat("LPAREN");
                while(true){
                    token wr=current_token;
                    string last_token=lex.last_token;
                    int pos=lex.pos;
                    char current_char=lex.current_char;
                    
                    bool bex=bool_expr();
                    eat("RPAREN");
                    if(bex){
                        eat("LKEY");
                        block();
                        eat("RKEY");
                        current_token=wr;
                        lex.last_token=last_token;
                        lex.pos=pos;
                        lex.current_char=current_char;
                    }else{
                        pass();
                        break;
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

