var gulp = require('gulp');
var concat = require('gulp-concat');
var uglify = require('gulp-uglify');
var del = require('del');
var minifyCSS = require('gulp-minify-css');
var bower = require('gulp-bower');
var sourcemaps = require('gulp-sourcemaps');

var config = {
    sources: {
        jquery: 'bower_components/jquery/dist/jquery.js',
        lodash: 'bower_components/lodash/dist/lodash.js',
        moment: 'bower_components/moment/moment.js',
        d3: 'bower_components/d3/d3.js',
        d3cloud: 'bower_components/d3-cloud/build/d3.layout.cloud.js',
        highcharts: 'bower_components/highcharts/highcharts.js',
        treemap: 'bower_components/highcharts/modules/treemap.js',
        datatables: 'bower_components/datatables/media/js/jquery.dataTables.js',
        datatablesCss: 'bower_components/datatables/media/css/jquery.dataTables.css',
        spinjs: 'bower_components/spin.js/spin.js',
        filesize: 'bower_components/filesize/lib/filesize.js'
    }
}

gulp.task('jquery', ['bower-restore'], function () {
    return gulp.src(config.sources.jquery)
        .pipe(gulp.dest('Scripts'))
        .pipe(sourcemaps.init())
        .pipe(uglify())
        .pipe(concat('jquery.min.js'))
        .pipe(sourcemaps.write('maps'))
        .pipe(gulp.dest('Scripts'));
});

gulp.task('datatables', ['bower-restore'], function () {
    return gulp.src(config.sources.datatables)
        .pipe(concat('datatables.js'))
        .pipe(gulp.dest('Scripts'))
        .pipe(sourcemaps.init())
        .pipe(uglify())
        .pipe(concat('datatables.min.js'))
        .pipe(sourcemaps.write('maps'))
        .pipe(gulp.dest('Scripts'));
});

gulp.task('datatablesCss', ['bower-restore'], function () {
    return gulp.src(config.sources.datatablesCss)
        .pipe(minifyCSS())
        .pipe(concat('datatables.css'))
        .pipe(gulp.dest('Content'));
});

gulp.task('d3', ['bower-restore'], function () {
    return gulp.src([config.sources.d3, config.sources.d3cloud])
        .pipe(gulp.dest('Scripts'))
        .pipe(sourcemaps.init())
        .pipe(concat('d3.cloud.all.js'))
        .pipe(sourcemaps.write('maps'))
        .pipe(gulp.dest('Scripts'));
});

gulp.task('lodash', ['bower-restore'], function () {
    return gulp.src(config.sources.lodash)
        .pipe(gulp.dest('Scripts'))
        .pipe(sourcemaps.init())
        .pipe(uglify())
        .pipe(concat('lodash.min.js'))
        .pipe(sourcemaps.write('maps'))
        .pipe(gulp.dest('Scripts'));
});

gulp.task('moment', ['bower-restore'], function () {
    return gulp.src(config.sources.moment)
        .pipe(gulp.dest('Scripts'))
        .pipe(sourcemaps.init())
        .pipe(uglify())
        .pipe(concat('moment.min.js'))
        .pipe(sourcemaps.write('maps'))
        .pipe(gulp.dest('Scripts'));
});

gulp.task('highcharts', ['bower-restore'], function () {
    return gulp.src([config.sources.highcharts, config.sources.treemap])
        .pipe(concat('highcharts.js'))
        .pipe(gulp.dest('Scripts'))
        .pipe(sourcemaps.init())
        .pipe(uglify())
        .pipe(concat('highcharts.min.js'))
        .pipe(sourcemaps.write('maps'))
        .pipe(gulp.dest('Scripts'));
});

gulp.task('dashboard', ['bower-restore'], function () {
    return gulp.src('Scripts/dashboard.js')
        .pipe(sourcemaps.init())
        .pipe(uglify())
        .pipe(concat('dashboard.min.js'))
        .pipe(sourcemaps.write('maps'))
        .pipe(gulp.dest('Scripts'));
});

gulp.task('spinjs', ['bower-restore'], function () {
    return gulp.src(config.sources.spinjs)
        .pipe(gulp.dest('Scripts'))
        .pipe(sourcemaps.init())
        .pipe(uglify())
        .pipe(concat('spinjs.min.js'))
        .pipe(sourcemaps.write('maps'))
        .pipe(gulp.dest('Scripts'));
});

gulp.task('filesize', ['bower-restore'], function () {
    return gulp.src(config.sources.filesize)
        .pipe(gulp.dest('Scripts'))
        .pipe(sourcemaps.init())
        .pipe(uglify())
        .pipe(concat('filesize.min.js'))
        .pipe(sourcemaps.write('maps'))
        .pipe(gulp.dest('Scripts'));
});

gulp.task('clean', function () {
    return del(['Scripts/*.min.js', 'Scripts/all.js']);
});

// Combine and the vendor files from bower into bundles (output to the Scripts folder)
gulp.task('scripts', ['clean', 'jquery', 'lodash', 'moment', 'highcharts', 'spinjs', 'filesize', 'datatables', 'dashboard'], function () {
    return gulp.src([config.sources.jquery, config.sources.lodash, config.sources.highcharts, config.sources.datatables, config.sources.treemap, config.sources.d3, config.sources.d3cloud, config.sources.spinjs, config.sources.filesize, 'Scripts/dashboard.js'])
        .pipe(concat('all.js'))
        .pipe(gulp.dest('Scripts'))
        .pipe(sourcemaps.init())
        .pipe(uglify())
        .pipe(concat('all.min.js'))
        .pipe(sourcemaps.write('maps'))
        .pipe(gulp.dest('Scripts'));
});

gulp.task('css', ['datatablesCss', 'bower-restore'], function () {
    return gulp.src('Content/dashboard.css')
     .pipe(minifyCSS())
     .pipe(concat('dashboard.min.css'))
     .pipe(gulp.dest('Content'));
});


//Restore all bower packages
gulp.task('bower-restore', function() {
    return bower();
});

//Set a default tasks
gulp.task('default', ['scripts', 'css'], function () {

});
