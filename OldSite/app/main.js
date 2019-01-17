requirejs.config({
    paths: {
        'text': '../lib/require/text',
        'durandal': '../lib/durandal/js',
        'plugins': '../lib/durandal/js/plugins',
        'transitions': '../lib/durandal/js/transitions',
        'knockout': '../lib/knockout/knockout-3.1.0',
        'bootstrap': '../lib/bootstrap/js/bootstrap',
        'jquery': '../lib/jquery/jquery-1.9.1',
        'json2': '../lib/json2/json2',
        'jstore': '../lib/jstorage/jstorage',
        'i18next': '../lib/i18/i18next',
        'moment': '../lib/moment/moment',
        'koswitch': '../lib/knockout/switch/knockout-switch-case'
    },
    shim: {
        'bootstrap': {
            deps: ['jquery'],
            exports: 'jQuery'
        }
    },
    waitSeconds: 0
});
define(
    'jstr', window.$.jStorage
);

define(['durandal/system', 'durandal/app', 'durandal/viewLocator', 'i18next'], function (system, app, viewLocator, i18next) {
    //>>excludeStart("build", true);
    system.debug(true);
    //>>excludeEnd("build");

    app.title = 'Martin Parker Portfolio';

    app.configurePlugins({
        router: true,
        dialog: true
    });

    app.start().then(function () {
        //Replace 'viewmodels' in the moduleId with 'views' to locate the view.
        //Look for partial views in a 'views' folder in the root.
        viewLocator.useConvention();

        i18next.init({
            lng: 'en',
            resources: {
                en: {
                    translation: {
                        'itmOpeningMsg': '<b>Pardon my dust</b><br>This development path was abandoned in favor of an Agular & Azure approach.<br>You can track the development of the new site <a href="https://github.com/mparker42/MartinParkerAngularCV">here</a>.'/*'This is a custom built portfolio site.<br>Help for navigating this site can be found by clicking the <i class="fa fa-question-circle" style="color:rgb(66, 144, 66)"></i> icon below.'*/,
                        'itmNeverShow': 'Never Show This Again',
                        'home': 'Home',
                        'itmProg1Desc': 'Programming 1 acted as my introduction to programming.<br>The module covered basics from variables and loops up to classes.<br>The first projects were built in notepad and later projects were built in Visual Studio 2010.',
                        'itmTagTypeLanguage': 'Language',
                        'itmTagTypeProjectStartDate': 'Project Start Date',
                        'itmTagTypeUniversityWork': 'University Work',
                        'itmTagTypeUniversityYear': 'University Year',
                        'itmTagTypeProjectName': 'Project Name',
                        'year1': 'Year 1',
                        'year2': 'Year 2',
                        'year4': 'Year 4',
                        'show': 'Show ',
                        'projectsWhere': 'projects where:',
                        'theProjectIs': 'The project is ',
                        'not': 'not ',
                        'the': 'The ',
                        'is': ' is ',
                        'isNot': 'is not',
                        'after': 'after ',
                        'before': 'before ',
                        'between': 'between ',
                        'afterNS': 'after',
                        'beforeNS': 'before',
                        'betweenNS': 'between',
                        'and': ' and ',
                        'everything': 'everything.',
                        'orderedBy': 'Order the projects by:',
                        'ordered': 'Ordered ',
                        'contains': ' contains ',
                        'excludes': ' excludes ',
                        'nothing': ' nothing.<br>',
                        'first': ' first',
                        'last': ' last',
                        'ascending': ' ascending',
                        'descending': ' descending',
                        'alphabeticallyAscending': ' alphabetically ascending',
                        'alphabeticallyDescending': ' alphabetically descending',
                        'itmProg2Desc': 'Programming 2 was a continuation of learning C#.<br>The module covered abstract data types, polymorphism and finally XNA.<br>I have linked the XNA solution for this site.<br>To run the solution you will need both visual studio 2010 and XNA',
                        'itmHangmanDesc': 'During the christmas break of university there was a challenge to create a hangman game within XNA. <br>This was completely optional and done as a joke by Tommy the lecturer. <br>I completed this over a few weeks in December and made all of the graphics myself.',
                        'itmDatabasesDesc': 'The databases module in my university course covered the basics of database design and SQL.<br>To my memory this was written in MySQL but I have spent the subsequent year working with SQL Server.<br>I have attached the final assessment for this module.',
                        'itmTagTypeProjectEndDate': 'Project End Date',
                        'itmTagTypeEngine': 'Engine',
                        'itmTagTypePackage': 'Package',
                        'itmGraphics1Desc': 'Graphics 1 was an introduction to graphics programming with C++ and OpenGL.<br>The module was split into two assignments. <br>The first assignment was a 2D GUI. The GUI works as a window opening/resizing programme. I have made some changes to the project so it can be run out of the box but only the release build can load the image files.<br>The second assignment is a 3D model viewer. The model viewer is a simple viewer which has rotation control.',
                        'itmNetworksAndSecurityDesc': 'The networks and security module consisted of mostly theory.<br>The assessment was split into a paper and a python application.<br>The files attached are the python application.',
                        'itmGraphics2Desc': 'Graphics 2 was a continuation of 3D rendering shown in Graphics 1.<br>The rendering used per pixel lighting and code manipulatable shader light sources.<br>I treated this as an exercise of efficiency.<br>The source project has issues with libraries so it does not build. When I have a chance later I will fix this so the solution runs out the box.',
                        'itmAppDevDesc': 'Application development was a module based around website development.<br>There were tutorials for both ASP.NET and PHP in the module but I chose to make the application an ASP.NET application.<br>The assessment project uses WebGL on a canvas to render a gallery.<br>The gallery loads custom rooms based off a saved seed.<br>The intention was to add drawing tools to the rooms for customisation.<br>Drawing would change the stored seed for the user to match the drawing.<br>If given the chance I would like to revisit this especially with the new resurgence of virtual reality.',
                        'itmTeamProjDesc': 'Team project as a module with a loose concept: Build a product within a team.<br>Our team ended up making a tool which encrypts downloads on google chrome browsers.<br>The encryption tool was added to the google play store under the name "FluffyPuffin".',
                        'itmSystemsProgrammingDesc': 'Systems Programming was based around building C++ development for systems without user interaction.<br>The assessment was to use the DirectSound API to build a music streaming application which has a 4 second rotary buffer for storing the music.<br>The challenge of this module was wrangling the DirectSound API into working.<br>I would love to revisit this module in the future; changing the api and changing the streaming method from TCP to UDP.',
                        'itmGameDevDesc': 'Game Development was an open ended module with the aim of developing a game demo.<br>I was placed into a team of 11 with 7 artists and 4 programmers.<br>The only restriction for the game was that it needed to have soundscapes as the main theme and use the sound library the module leaders provided.<br>The finished game was a stealth game with platforming elements.<br>The game was built in Unreal Engine 4.10 and acted as my introduction to building games in an engine instead of from scratch.',
                        'itmGameBehavDesc': 'Game Behaviour was a module covering AI and Phsyics in games.<br>For the assesment I made a game in MonoGame which is a non-microsoft continuation of XNA.',
                        'itmIndiStudDesc': 'Independent Studies is the module name for the dissertation in my BSc.<br>For my dissertation I decided to see if a free-to-play game could use positive actions for currency.<br>The produced game was put on the google play store under the name EnergyRace.',
                        'searchCriteria': 'Search Criteria',
                        'orderCriteria': 'Order Criteria',
                        'criteria': ' Criteria',
                        'searchFor': 'Search for: ',
                        'whereTheValue': 'Where the value ',
                        'with': ' with ',
                        'whereTheValueIs': 'Where the value is ',
                        'whereTheDateIs': 'Where the date is ',
                        'whereTheDatesAre': 'Where the dates are ',
                        'projectsWithTheTag': 'projects which are ',
                        'including': 'Including ',
                        'excluding': 'Excluding ',
                        'whereTheTag': 'Where the tag ',
                        'includes': 'includes',
                        'excludes': 'excludes',
                        'done': 'Done',
                        'orderBy': 'Order by',
                        'haveProjectsContainingTheTagOrdered': 'Have projects containing the tag ordered ',
                        'andTheValueIs': 'And the following value is',
                        'projectsWhichAre': 'Projects which are '
                    }
                }
            }
        }, function(err, t) {
            //Show the app by setting the root view model for our application with a transition.
            app.setRoot('viewmodels/shell', 'entrance');
        });
    });
});