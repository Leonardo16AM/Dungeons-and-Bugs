using Telegram.Bot;
using System;
using System.Threading;

class interpreter: ICloneable{
    public Dictionary<string,string>context,inner_context;
    public List<string> actions;
    public lexer lex;
    token current_token;
    public string code;
    ITelegramBotClient botClient;
    IClient Client;
    List<int>chat_ids;
    string to_ret;
    public bool ret_int=false;
    public bool root =true;
    System.Random random;

    public interpreter(IClient client,string code,Dictionary<string,string>context,List<int>chat_ids){
        this.Client=client;
        this.chat_ids=chat_ids;
        this.code=code;
        this.context=new Dictionary<string, string>();
        this.inner_context=new Dictionary<string, string>();
        this.actions=new List<string>();
        this.random = new System.Random();

        List<string>vars=new List<string>();
        if(context!=null){
            this.context=context;
            vars=new List<string>(context.Keys);
        }
        if(code=="")code=";";
        lex=new lexer(code,vars);
        this.current_token=lex.get_next_token();
    }
    public object Clone(){
        return new interpreter(this.Client,this.code,this.context,this.chat_ids); 
    }
    public object CloneC(string script){
        return new interpreter(this.Client,script,this.context,this.chat_ids); 
    }

    public void error(string e){throw new Exception(e);}







    public void add_var(string key,string value){
        lex.Addvar(key);
        Console.WriteLine("ADDING VAR: "+key+" "+value);
        if(root){
            if(context.ContainsKey(key))
                context[key]=value;
            else
                context.Add(key,value);
        }else{
            if(inner_context.ContainsKey(key))
                inner_context[key]=value;
            else
                inner_context.Add(key,value);
        }
    }

    void modify_var(string key,string value){
        if(!context.ContainsKey(key) && !inner_context.ContainsKey(key)){
            add_var(key,value);
        }else{
            if(context.ContainsKey(key))
                context[key]=value;
            else
                inner_context[key]=value;
        }
    }
    string var_value(string key){
        if(context.ContainsKey(key))
            return context[key];
        if(inner_context.ContainsKey(key))
            return inner_context[key];
        error("Interpreter Error: Variable "+key+" not found");
        return "";
    }
    public void print_context(){
        Console.WriteLine("===========Printing context============");
        foreach (var entry in context){
            Console.WriteLine(entry.Key+" "+entry.Value);
        }
        Console.WriteLine("===========Printing inner context============");
        foreach (var entry in inner_context){
            Console.WriteLine(entry.Key+" "+entry.Value);
        }
        Console.WriteLine("=======================================");
    }







    void eat(string token_type){
        if(current_token.type==token_type)
            current_token=lex.get_next_token();
        else
            error($"Interpreter Error: Expected {token_type} found {current_token.type}");
    }



    string rand(){
        eat("RND");
        eat("LPAREN");
        int val=int_expr();
        eat("RPAREN");
        return random.Next(val).ToString();
    }

    int int_factor(){
        token token = current_token;
        
        if(token.type == "INTEGER"){
            eat("INTEGER");
            return int.Parse(token.value);
        }
        if(token.type == "RND"){
            return int.Parse(rand());
        }
        if(token.type == "VAR"&& var_value(token.value)[0]!='>'){
            string vname=token.value;
            eat("VAR");
            return int.Parse(var_value(vname));
        }
        if(token.type == "VAR" && var_value(token.value)[0]=='>'){
            string r=run_function();
            return int.Parse(r);
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
            }
            if(token.type=="DIV"){
                eat("DIV");
                result/=int_factor();
            }
            if(token.type=="MOD"){
                eat("MOD");
                result%=int_factor();
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
            result=rand();
        }
        if(current_token.type=="VAR"&& var_value(current_token.value)[0]!='>'){
           result+=var_value(current_token.value).ToString();
           eat("VAR");
        }
        if(current_token.type=="VAR"&& var_value(current_token.value)[0]=='>'){
           result+=run_function().ToString();
        }
        return result;
    }
    string str_expr(){
        string result=str_term();
        while(current_token.type!="COMA" && current_token.type!="SCOL" && current_token.type!= "RPAREN"&& current_token.type!= "LPAREN"){
            token token = current_token;
            if(token.type=="PLUS"){
                eat("PLUS");
                result+=str_term();
            }
        }
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
        if(current_token.type=="NOT"){
            eat("NOT");
            return !bool_expr();
        }
        if(current_token.type=="LPAREN" && is_bool() ){
            eat("LPAREN");
            bool ret=bool_expr();
            eat("RPAREN");
            return ret;
        }
        if(current_token.type=="EXST" ){
            bool ret=false;
            eat("EXST");
            eat("LPAREN");
            string v=str_expr();
            if(context.ContainsKey(v) || inner_context.ContainsKey(v))ret=true;
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







    
    class function{
        
        public interpreter interp;
        public List<string>pms;
        public string type="v";

        public function(interpreter interp,string obj){
            this.pms=new List<string>();
            List<string>deobj=new List<string>(obj.Split('~'));
            this.type=deobj[0].Substring(1);
            for(int i=1;i<deobj.Count-1;i++)
                pms.Add(deobj[i]);
            
            this.interp=(interpreter)interp.CloneC(deobj[deobj.Count-1].Substring(2));
            if(type=="i")this.interp.ret_int=true;
            this.interp.root=false;
        }
        public void Add(string key,string value){
            this.interp.add_var(key,value);
            this.interp.lex.vars.Add(key);
        }
        public string run(){
            return this.interp.run();
        }    
    };
    string run_function(){
        string fname=current_token.value;
        eat("VAR");
        eat("LPAREN");
        
        function f=new function(this,context[fname]);
        foreach(string p in f.pms){
            string pname=p.Substring(2);
            if(p[0]=='s')
                f.Add(pname,str_expr());
            if(p[0]=='i')
                f.Add(pname,int_expr().ToString());
            if(current_token.type=="COMA")eat("COMA");
        }
        eat("RPAREN"); 
        string ret=f.run();
        this.context=f.interp.context;
        return ret;
    }








    void line(){
        token token = current_token;
        
        if(token.type=="NOTI"){//Notification
            eat("NOTI");
            eat("LPAREN");
            Client.notify(
                chat_ids,
                new ClientParams(str_expr())
            );
            eat("RPAREN");
        }      
        if(token.type=="NOTIP"){//Notification with picture
            eat("NOTIP");
            eat("LPAREN");
            string url=str_expr();
            eat("COMA");
            Client.notify(
                chat_ids,
                new ClientParams(str_expr(), pU: url)
            );
            eat("RPAREN");
        }
        if(token.type=="ADDP"){//Add power to player
            eat("ADDP");
            eat("LPAREN");
            string user=str_expr();
            eat("COMA");
            string p_name=str_expr();
            eat("COMA");
            string p_desc=str_expr();
            eat("COMA");
            string p_script=current_token.value;
            eat("SCRIPT");
            actions.Add($"add%{user}%{p_name}%{p_desc}%{p_script}");
            eat("RPAREN");
        }
        
        if(token.type=="DELP"){//Remove power from player player
            eat("DELP");
            eat("LPAREN");
            string user=str_expr();
            eat("COMA");
            string p_name=str_expr();
            actions.Add($"del%{user}%{p_name}");
            eat("RPAREN");
        }
        
        if(token.type=="INT"){//Integer declaration
            eat("INT");
            string vname=current_token.value;
            Console.WriteLine(vname+" lllllllllllll ");
            eat("VAR");
            eat("ASG");
            int value=int_expr();
            add_var(vname,value.ToString());
        }
        if(token.type=="VAR" && var_value(token.value)[0]!='>' ){//Integrer modification
            string vname=current_token.value;
            eat("VAR");
            if(current_token.type=="ASG"){
                eat("ASG");
                int value=int_expr();
                modify_var(vname,value.ToString());
            }else{
                if(current_token.type=="PLPL"){
                    eat("PLPL");modify_var(vname,(int.Parse(var_value(vname))+1).ToString());
                }
                if(current_token.type=="MNMN"){
                    eat("MNMN");modify_var(vname,(int.Parse(var_value(vname))-1).ToString());
                }
            }
        }
        if(token.type=="SLEEP"){//sleep
            eat("SLEEP");
            eat("LPAREN");
            Thread.Sleep(int_expr());
            eat("RPAREN");
        }
        if(token.type=="ENDT"){//End turn
            eat("ENDT");
            eat("LPAREN");
            modify_var("G_endturn","1");
            eat("RPAREN");
        } 
        if(token.type=="CTO"){//End turn
            eat("CTO");
            eat("LPAREN");
            if(var_value("G_TO")=="1")
                modify_var("G_TO","-1");
            else
                modify_var("G_TO","1");
            eat("RPAREN");
        } 
        if(token.type=="VAR" && var_value(token.value)[0]=='>'){//Calling a void function
            run_function();
        }
        if(token.type=="RET"){//Return something
            eat("RET");
            if(token.type=="SCOL"){to_ret="";return;}
            if(ret_int)
                to_ret=int_expr().ToString();
            else
                to_ret=str_expr();
        }   
        eat("SCOL");      
    }
    void pass(){
        int cnt=1;
        while(cnt!=0){
            string type=current_token.type;
            if(type=="EOF")return;
            if(type=="LKEY")cnt++;
            if(type=="RKEY")cnt--;
            eat(type);
        }
    }
    string block(){
        while(current_token.type!="RKEY" && current_token.type!="EOF" ){
            token token = current_token;
            
            if(token.type=="IF"){
                eat("IF");
                eat("LPAREN");
                bool bex=bool_expr();
                eat("RPAREN");
                eat("LKEY");
                if(bex){
                    string ret=block();
                    if(ret!="EMPT")return ret;
                    eat("RKEY");
                }else pass();
                if(current_token.type=="ELSE"){
                    eat("ELSE");
                    eat("LKEY");
                    if(!bex){
                        string ret=block();
                        if(ret!="EMPT")return ret;
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
                        string ret=block();
                        if(ret!="EMPT")return ret;
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
            if(token.type=="FOR"){
                eat("FOR");
                eat("LPAREN");
                line();
                while(true){
                    token wr=current_token;
                    string last_token=lex.last_token;
                    int pos=lex.pos;
                    char current_char=lex.current_char;
                    
                    bool bex=bool_expr();
                    eat("RPAREN");
                    if(bex){
                        eat("LKEY");
                        string ret=block();
                        if(ret!="EMPT")return ret;
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
            if(token.type=="DEF"){                
                eat("DEF");
                string func="";
                if(current_token.type=="VOID"){eat("VOID");func=">v";}                
                if(current_token.type=="INT"){eat("INT");func=">i";}
                if(current_token.type=="STR"){eat("STR");func=">s";}
                string name=current_token.value;
                eat("VAR");
                eat("LPAREN");
                while(current_token.type!="RPAREN"){
                    if(current_token.type=="INT"){eat("INT");func+="~i:";}
                    if(current_token.type=="STR"){eat("STR");func+="~s:";}
                    string vname=current_token.value;
                    eat("VAR");
                    func+=vname;
                    if(current_token.type=="COMA")eat("COMA");
                }
                func+="~c:";
                eat("RPAREN");
                eat("LKEY");
                int cnt=1;
                while(cnt!=0){
                    if(current_token.type=="LKEY"){cnt++;}
                    if(current_token.type=="RKEY"){cnt--;}
                    if(cnt!=0){
                        if(current_token.type=="STRING")
                           func+=" \""+current_token.value+"\" ";
                        else
                           func+=" "+current_token.value+" ";
                        eat(current_token.type);
                    }
                }
                eat("RKEY");
                try{
                    context.Add(name,func);
                }catch{
                    error($"A function called {name} already exist");
                }
                continue;
            }
            line();
            if(to_ret!=null){
                Console.WriteLine("RETURNING "+to_ret);
                return to_ret;
            }
        }
        return "EMPT";
    }

    public string run(){
        Console.WriteLine("RUNNING :"+ code);
        return block();
    }

}

