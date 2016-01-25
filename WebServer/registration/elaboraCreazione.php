<?php

include_once($_SERVER['DOCUMENT_ROOT']."/config.php");

if(isset($_REQUEST['user']) && $_REQUEST['user'] != "")
{
    $username = strtoupper($_REQUEST['user']);
    if(isset($_REQUEST['psw']) && $_REQUEST['psw'] != "")
    {
        $password = strtoupper($_REQUEST['psw']);
        if(isset($_REQUEST['email']) && $_REQUEST['email'] != "")
        {
            $email = strtoupper($_REQUEST['email']);
        }
        else
            die("Non sei autorizzato a visualizzare questa pagina");
    }
    else
        die("Non sei autorizzato a visualizzare questa pagina");
}
else
{
    die("Non sei autorizzato a visualizzare questa pagina");
}

$sha_hash_pass = strtoupper(sha1($username.":".$password));

$db = new db_class();
$res = $db->creaAccount($username, $sha_hash_pass, $email);
echo $res;

?>