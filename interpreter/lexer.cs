class token{
    public string type,value;
    public token(string type,string value){
        this.type = type;
        this.value = value;
    }
}

class lexer{
    List<string>token_names=new List<string>(){"PLPL","MNMN","GET","LET","DIF","EQ","PLUS","MINUS","MUL","DIV","ASG","LPAREN","RPAREN","LKEY",
    "RKEY","SCOL","AND","OR","GT","LT","NOTI","IF","ELSE","WHILE","NOTIP","COMA","SLEEP","ADDP","DELP","ENDT","RET","DEF","RCOR",
    "LCOR","FOR","EXST","MOD","NOT","CTO"};
    List<string>token_string=new List<string>(){"++","--",">=","<=","!=","==","+","-","*","/","=","(",")","{","}",";","&","|",">","<","notify",
        "if","else","while","notipic",",","sleep","add_power","del_power","end_turn","return","def","[","]","for","exist","%","!","change_turn_order"};
    public string text,last_token;
    public int pos;
    public char  current_char;
    public List<string>vars;

    public lexer(string text,List<string>vars){
        this.text=text;
        this.pos=0;
        this.current_char=text[pos];
        this.vars=vars;
        this.last_token="";
    }
    public void Addvar(string s){vars.Add(s);}

    public void error(string e){throw new Exception(e);}
    public void advance(){
        pos++;
        if(pos>text.Length-1)
            current_char='#';
        else
            current_char=text[pos];
    }
    public void skip(){
        if(same_token("/*"))
            while(!same_token("*/"))advance();
        
        while(current_char!='#' && (current_char==' ' || current_char=='\t' || current_char=='\n' || current_char=='\r') )
            advance();
    }

    public bool same_token(string s){
        int wr=0;
        bool can=true;
        while(wr<s.Count()){
            if(current_char!=s[wr]){can=false;break;}
            advance();
            wr++;
        }
        if(!can){
            while(wr!=0){
                wr--;
                pos--;
            }
            if(pos>text.Length-1)
                current_char='#';
            else
                current_char=text[pos];
        }else{ 
            last_token=s;
        }
        return can;
    }


    bool is_varname(){
        for(int i=0;i<vars.Count();i++){
            Console.WriteLine(vars[i]);
            if(same_token(vars[i]) )
                return true;
        }
        return false;
    }



    public int integer(){
        string result="";
        while(current_char>='0' && current_char<='9'){
            result+=current_char;
            advance();
        }
        return int.Parse(result);
    }

    public string strin(){
        string result="";
        advance();
        while(current_char!='"' && current_char!='#'){
            result+=current_char;
            advance();
        }
        Console.WriteLine("lll "+result);
        advance();
        return result;
    }
    public string script(){
        string result="";
        advance();
        while(current_char!='\'' && current_char!='#'){
            result+=current_char;
            advance();
        }
        advance();
        return result;
    }


    string get_var(){
        string ret="";
        while(current_char!='#'&&current_char!='='&&current_char!=' '&&current_char!='\t'&&current_char!=';'&& current_char!=',' && current_char!=')'&& current_char!='('&& current_char!='='&& current_char!='{'&& current_char!='}'&& current_char!='['&& current_char!=']' ){
            ret+=current_char;
            advance();
        }
        return ret;
    }

    public token get_next_token(){
        while(current_char!='#'){
            skip();
            if(current_char=='#')return new token("EOF","#");
            
            if(last_token=="int" || last_token=="str" || last_token=="void"){
                string vname=get_var();
                vars.Add(vname);
                last_token="";
                return new token("VAR",vname);        
            }
            
            if(current_char>='0' && current_char<='9')
                return new token("INTEGER",integer().ToString());
            if(current_char=='"')
                return new token("STRING",strin());
            if(current_char=='\'')
                return new token("SCRIPT",script());

            for(int i=0;i<token_names.Count();i++)
                if(same_token(token_string[i]))
                    return new token(token_names[i],token_string[i].ToString());
                
            if(same_token("str")){
                last_token="str";
                return new token("STR","str");
            }
            if(same_token("int")){
                last_token="int";
                return new token("INT","int");
            }
            if(same_token("void")){
                last_token="void";
                return new token("VOID","void");
            }
            if(same_token("random")){
                return new token("RND","random");
            }
            if(is_varname()){
                return new token("VAR",last_token);
            }
            error("Parser error: Invalid character :"+current_char);
        }
        return new token("EOF","#");
    }
}