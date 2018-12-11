var express = require("express");
var router = express.Router();

/* GET home page. */
router.get("/", function(req, res, next) {
  // res.send("hello dear mine");
  res.render("index", { title: "New Ways" });
});

module.exports = router;
