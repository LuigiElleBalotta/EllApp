<?php
include_once($_SERVER['DOCUMENT_ROOT']."/config.php");

if(isset($_REQUEST['username']))
{
    $username = $_REQUEST['username'];
    if(isset($_REQUEST['password']))
    {
        $password = $_REQUEST['password'];
        $db = new db_class();
        if($db->validaLogin($username, $password))
        {
            $db->setLastIP($username);
            $db->setLastLogin($username);
            echo "1";
        }
        else
        {
            echo "2";
        }
    }
    else
    {
        echo "3";
    }
}
else
{
    echo "4";
}
?>