<?php
include_once($_SERVER['DOCUMENT_ROOT']."/config.php");

if(isset($_REQUEST['username']))
{
    $username = $_REQUEST['username'];
    $db = new db_class();
    $accID = $db->getAccountId($username);
    echo $db->GetPendingRequests($accID);
}
else
{
    echo "No username.";
}
?>