<?php
include_once($_SERVER['DOCUMENT_ROOT']."/config.php");

if(isset($_REQUEST['username']))
{
    $username = $_REQUEST['username'];
    if(strlen($username) >= 3)
    {
        $db = new db_class();
        echo json_encode($db->GetAccountStartWith($username));
    }
}
else
{
    echo "1";
}
?>