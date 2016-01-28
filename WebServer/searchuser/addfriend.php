<?php
include_once($_SERVER['DOCUMENT_ROOT']."/config.php");

if(isset($_REQUEST['from']))
{
    $fromUser = $_REQUEST['from'];
    if(isset($_REQUEST['idTo']))
    {
        $idTo = $_REQUEST['idTo'];
        $db = new db_class();
        $p1ID = $db->getAccountId($fromUser);
        if($p1ID != $idTo)
        {
            if($db->addFriend($p1ID, $idTo))
                echo "1";
            else
                echo "4";
        }
        else
            echo "5";
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
?>