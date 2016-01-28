<?php
include_once($_SERVER['DOCUMENT_ROOT']."/config.php");

if(isset($_REQUEST['person2']))
{
    $me = $_REQUEST['person2'];
    if(isset($_REQUEST['person1']))
    {
        $who = $_REQUEST['person1'];
        $db = new db_class();
        $meID = $db->getAccountId($me);
        if($meID != $who)
        {
            if($db->acceptFriend($meID, $who))
                echo "1"; //ok
            else
                echo "2"; //Error
        }
        else
            echo "5"; //Can't accept myself
    }
    else
    {
        echo "4"; //No WHO
    }
}
else
{
    echo "3"; //No ME
}
?>