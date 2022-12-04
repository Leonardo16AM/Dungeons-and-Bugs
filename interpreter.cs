using Telegram.Bot;

class token{
    public string type,value;
    public token(string ty,string val){
        type = ty;
        value = val;
    }
}

class parser{
    string text;
    int pos;
    char  current_char;
    public parser(string s){
        text=s;
        pos=0;
        current_char=text[pos];
    }
    public void error(){
        throw new Exception("Invalid character");
    }
    public void advance(){
        pos++;
        if(pos>text.Length-1)
            current_char='#';
        else
            current_char=text[pos];
    }
    public void skip(){
        while(current_char!='#' && (current_char==' ' || current_char=='\t' || current_char=='\n' || current_char=='\r') )
            advance();
    }

    public int integer(){
        string result="";
        while(current_char!='#' && current_char>='0' && current_char<='9'){
            result+=current_char;
            advance();
        }
        return int.Parse(result);
    }

    public string strin(){
        string result="";
        while(current_char!='"' && current_char!='#'){
            result+=current_char;
            advance();
        }
        advance();
        return result;
    }

    public token get_next_token(){
        while(current_char!='#'){
            if(current_char==' ' || current_char=='\t' || current_char=='\n' || current_char=='\r')
                skip();
            if(current_char>='0' && current_char<='9')
                return new token("INTEGER",integer().ToString());
            
            if(current_char=='"')
                return new token("STRING",strin());

            if(current_char=='+'){
                advance();
                return new token("PLUS","+");
            }
            if(current_char=='-'){
                advance();
                return new token("MINUS","-");
            }
            if(current_char=='*'){
                advance();
                return new token("MUL","*");
            }
            if(current_char=='/'){
                advance();
                return new token("DIV","/");
            }
            if(current_char=='('){
                advance();
                return new token("LPAREN","(");
            }
            if(current_char==')'){
                advance();
                return new token("RPAREN",")");
            }
            if(current_char=='{'){
                advance();
                return new token("LKEY","{");
            }
            if(current_char=='}'){
                advance();
                return new token("RKEY","}");
            }
            if(current_char==';'){
                advance();
                return new token("SCOL",";");
            }
            
            error();
        }
        return new token("EOF","#");
    }
}

class interpreter{
    Dictionary<string,int>context;
    parser lex;
    token current_token;
    string code;
    ITelegramBotClient botClient;

    public interpreter(ITelegramBotClient botClient,string s,Dictionary<string,int>cont=null){
        this.botClient=botClient;
        code=s;
        context=new Dictionary<string, int>();
        if(cont!=null)context=cont;
        lex=new parser(code);
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
}

