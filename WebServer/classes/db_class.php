<?php
include_once($_SERVER['DOCUMENT_ROOT']."/config.php");
class db_class
{
    private $conn;
    
    function __construct() {
        $this->conn = new mysqli(MYSQL_HOST, MYSQL_USERNAME, MYSQL_PASSWORD, MYSQL_DATABASE) or die("error connect to database");
    }
    
    public function validaLogin($username, $password)
    {
        $username = strtoupper($username);
        $password = strtoupper($password);
        $sha_pass_hash = strtoupper(sha1($username.":".$password));
        $query = $this->conn->prepare("SELECT * FROM accounts WHERE username = ? AND password = ?;");
        $query->bind_param("ss", $username, $sha_pass_hash);
        $query->execute();
        $query->store_result();
        if($query->num_rows == 1)
        {
            $query->close();
            return true;
        }
        else
        {
            $query->close();
            return false;
        }
    }
    
    /*public function cambiaPasswordAccount($username, $oldPassword, $newPassword)
    {
        $oldShaPassHash = strtoupper(sha1($username.":".$oldPassword));
        if($this->validaLogin($username, $oldPassword))
        {
            $newShaPassHash = strtoupper(sha1($username.":".$newPassword));
            $query = $this->conn->prepare("UPDATE accounts SET password = ? WHERE username = ?;");
            $query->bind_param("ss", $newShaPassHash, $username);
            if($query->execute())
            {
                $query->close();
                echo "Password aggiornata con successo";
                return true;
            }
            else
            {
                $query->close();
                echo "Non sono riuscito a aggiornare la password";
                return false;
            }
        }
    }*/
    
    /*public function getAccountLevel($username)
    {
        $cod = 0;
        $query = $this->conn->prepare("SELECT ksLivello FROM account_access JOIN accounts ON ksAccount = idAccount WHERE username = ?");
        $query->bind_param("s", $username);
        $query->execute();
        $query->bind_result($livello);
        while($query->fetch())
            $cod = $livello;
        return $cod;
    }*/
    
    public function getAccountId($username)
    {
        $id = 0;
        $query = $this->conn->prepare("SELECT idAccount FROM accounts WHERE username = ?;");
        $query->bind_param("s", $username);
        $query->execute();
        $query->bind_result($idacc);
        while($query->fetch())
            $id = $idacc;
        $query->close();
        return $id;
    }
    
    public function creaAccount($username, $sha_hash_pass, $email)
    {
        $res = -1;
        $query = $this->conn->prepare("INSERT INTO accounts(username, password, email) VALUES (?, ?, ?);");
        $query->bind_param("sss", $username, $sha_hash_pass, $email);
        if($query->execute())
            $res = 1;
        else
            $res = 0;
        $query->close();
        return $res;
    }
    
    /*public function cancellaAccount($username)
    {
        $username = $_SESSION['cod_cliente']."-".$username;
        $arrayid = array();
        $query = $this->conn->prepare("SELECT idAccount FROM accounts WHERE username = ?;");
        $query->bind_param("s", $username);
        $query->execute();
        $query->bind_result($id);
        $c = 0;
        while($query->fetch())
        {
            $arrayid[$c] = $id;
            $c++;
        }
        $query->close();
        if(count($arrayid) > 1)
            $ids = implode(",", $arrayid);
        else
            $ids = $arrayid[0];
        $query = $this->conn->prepare("DELETE FROM account_access WHERE ksAccount IN($ids);");
        $query->execute();
        $query->close();
        $query = $this->conn->prepare("DELETE FROM accounts WHERE username LIKE ?;");
        $query->bind_param("s", $username);
        $query->execute();
        $query->close();
    }*/
    
    /*private function GetBetween($var1="",$var2="",$pool){
        $temp1 = strpos($pool,$var1)+strlen($var1);
        $result = substr($pool,$temp1,strlen($pool));
        $dd=strpos($result,$var2);
        if($dd == 0){
        $dd = strlen($result);
        }
        
        return substr($result,0,$dd);
    }*/
    
    public function setLastIP($username)
    {
        $ip = $_SERVER['REMOTE_ADDR'];
        $query = $this->conn->prepare("UPDATE accounts SET last_ip = ? WHERE username = ?;");
        $query->bind_param("ss", $ip, $username);
        $query->execute();
        $query->close();
    }
    
    public function setLastLogin($username)
    {
        $query = $this->conn->prepare("UPDATE accounts SET last_connection = '".date("Y-m-d H:i:s")."' WHERE username = '$username';");
        $query->execute();
        $query->close();
    }
    
    public function GetAccountStartWith($str)
    {
        $str = strtoupper("{$str}%");
        $query = $this->conn->prepare("SELECT idAccount, username FROM accounts WHERE username LIKE ?;");
        $query->bind_param("s", $str);
        $query->execute();
        $array = array();
        $c = 0;
        $query->bind_result($id, $username);
        while($query->fetch())
        {
            $array[$c]["id"] = $id;
            $array[$c]["name"] = $username;
            $c++;
        }
        $query->close();
        return $array;
    }
    
    public function addFriend($person1, $person2)
    {
        //First i get person1 ID
        $p1ID = $person1;
        $p2ID = $person2;
        
        $query = $this->conn->prepare("SELECT * FROM friendships WHERE (person1 = ? AND person2 = ?) OR (person1 = ? AND person2 = ?);");
        $query->bind_param("iiii", $p1ID, $p2ID, $p2ID, $p1ID);
        $query->execute();
        $query->store_result();
        if($query->num_rows == 0)
        {
            $query->close();
            $query = $this->conn->prepare("INSERT INTO friendships(person1, person2, isPending) VALUES(?, ?, 1);");
            $query->bind_param("ii", $p1ID, $p2ID);
            $query->execute();
            $query->close();
            return true;
        }
        else
        {
            $query->close;
            return false;
        }
    }
    
    public function GetPendingRequests($accID)
    {
        $query = $this->conn->prepare("SELECT * FROM friendships WHERE person2 = ? AND isPending = 1;");
        $query->bind_param("i", $accID);
        $query->execute();
        $query->store_result();
        $requests = $query->num_rows;
        $query->close();
        if($requests == 0)
            $requests = "";
        return $requests;
    }
    
    public function GetPendingRequestsSpecs($accID)
    {
        $array = array();
        $c = 0;
        $query = $this->conn->prepare("SELECT idAccount, username FROM friendships JOIN accounts ON person1 = idAccount WHERE person2 = ? AND isPending = 1;");
        $query->bind_param("i", $accID);
        $query->execute();
        $query->store_result();
        $query->bind_result($id, $username);
        $requests = $query->num_rows;
        if($requests > 0)
        {
            while($query->fetch())
            {
                $array[$c]["id"] = $id;
                $array[$c]["username"] = $username;
                $c++;
            }
        }
        $query->close();
        return $array;
    }
    
    public function acceptFriend($me, $who)
    {
        $bool = false;
        $query = $this->conn->prepare("UPDATE friendships SET isPending = 0 WHERE person1 = ? AND person2 = ?;");
        $query->bind_param("ii", $who, $me);
        if($query->execute())
            $bool = true;
        $query->close();
        return $bool;
    }
    
    public function refuseFriend($me, $who)
    {
        $bool = false;
        $query = $this->conn->prepare("UPDATE friendships SET isPending = 2 WHERE person1 = ? AND person2 = ?;");
        $query->bind_param("ii", $who, $me);
        if($query->execute())
            $bool = true;
        $query->close();
        return $bool;
    }
    
    public function GetFriendList($accID)
    {
        $array = array();
        $c = 0;
        $query = $this->conn->prepare("SELECT idAccount, username FROM friendships JOIN accounts ON person1 = idAccount WHERE person2 = ? AND isPending = 0;");
        $query->bind_param("i", $accID);
        $query->execute();
        $query->store_result();
        $query->bind_result($id, $username);
        $requests = $query->num_rows;
        if($requests > 0)
        {
            while($query->fetch())
            {
                $array[$c]["id"] = $id;
                $array[$c]["username"] = $username;
                $c++;
            }
        }
        $query->close();
        return $array;
    }
}

?>