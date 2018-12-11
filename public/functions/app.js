var createError = require("http-errors");
var express = require("express");
var path = require("path");
var cookieParser = require("cookie-parser");
var logger = require("morgan");
const proxy = require("http-proxy-middleware");

var indexRouter = require("./routes/index");
var usersRouter = require("./routes/users");

var app = express();
      
app.use(
  "/v0",(req, res, next)=>{
    if(req.originalUrl.indexOf('images/') >= 0){
      // due to the error on url encoding
      return res.redirect(req.originalUrl.replace('images/', 'images%2F'));
    }
      next();
  },
  proxy({
    target: "https://firebasestorage.googleapis.com",
    changeOrigin: true
  })
);

// view engine setup
app.set("views", path.join(__dirname, "views"));
app.set("view engine", "ejs");

app.use(logger("dev"));
app.use(express.json());
app.use(express.urlencoded({ extended: false }));
app.use(cookieParser());
app.use(express.static(path.join(__dirname, "public")));

app.use("/", indexRouter);
app.use("/users", usersRouter);

// catch 404 and forward to error handler
app.use(function(req, res, next) {
  next(createError(404));
});

// error handler
app.use(function(err, req, res, next) {
  // set locals, only providing error in development
  res.locals.message = err.message;
  res.locals.error = req.app.get("env") === "development" ? err : {};

  // render the error page
  res.status(err.status || 500);
  res.render("error");
});

module.exports = app;
