#addin nuget:?package=Cake.Docker&version=0.10.1
#addin nuget:?package=Cake.http&version=0.7.0

var target = Argument ("target", "Default");
var configuration = Argument ("configuration", "Release");
var project = Argument("project", string.Empty);

var outputPath = "./Publish/";
var projects = new string[] {  "Origine.Host" , "Origine.WebApi" , "Origine.Dashboard"};
var testProjects = GetFiles ("./tests/**/*.csproj");
var packOutPath = "./Publish/packages";
var packProjects = GetFiles("./src/Origine.*/*.csproj");
var harborUrl = "registry.yongegames.com";
var registry = $"{harborUrl}/cluster/";

var nugetUrl = "http://nuget.yongegames.com";
var nugetApiKey = "7bc6998c-5f00-4e9f-ab7e-b909c38a1f07";

var jenkinsUrl = "http://jenkins.yongegames.com";
var jenkinsToken = "d32a24d5-4107-46f9-a380-4e4dfbd11a9c";

int TimeStamp() => (int)((DateTime.UtcNow - new DateTime(2019, 11, 15)).TotalMinutes);
string latest = nameof(latest);

//清理工程
Task ("Clean")
    .Does (() => {
        if (DirectoryExists (outputPath))
            DeleteDirectory (outputPath, new DeleteDirectorySettings{
               Force = true,
               Recursive = true
            });
    });

//单元测试
Task ("Test")
    .Description ("Test")
    .Does (() => {
        var testSettings = new DotNetCoreTestSettings {
            NoRestore = true,
            Configuration = configuration
        };
        foreach (var project in testProjects) {
            DotNetCoreTest (project.FullPath, testSettings);
        }
    });

//添加包源
Task ("AddSource")
    .Does(()=>{
        var path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData);
        var nugetConfigPath = $"{path}/Nuget/Nuget.Config";
        try 
        {
            NuGetSetApiKey(nugetApiKey, nugetUrl, new NuGetSetApiKeySettings{ Verbosity = NuGetVerbosity.Detailed , ConfigFile = new FilePath(nugetConfigPath)});
        } catch(Exception) {} finally {
            NuGetAddSource("Origine.Nuget", nugetUrl);
        }
    });


//批量发布所有公用包
Task ("Pack")
    .IsDependentOn("Clean")
    .Does (() => {
        var settings = new DotNetCorePackSettings {
            Configuration = configuration,
            OutputDirectory = packOutPath,
            //VersionSuffix = $"{TimeStamp()}",
        };
        foreach (var project in packProjects) {
            DotNetCorePack (project.FullPath, settings);
        }
        var pushSetting = new DotNetCoreNuGetPushSettings {
            Source = nugetUrl,
            ApiKey = nugetApiKey
        };
        var packages = GetFiles ($"{packOutPath}/*.nupkg");
        foreach (var package in packages) {
            //DotNetCoreNuGetPush (package.FullPath, pushSetting);
        }
    });

//发布到本地
Task ("Publish")
    .Does(() => {
        var publishSettings = new DotNetCorePublishSettings {
            Force = true,
            Configuration = configuration,
        };
        foreach (var project in projects) {
            Information ($"Publish {project}!");
            publishSettings.OutputDirectory = new DirectoryPath ($"{outputPath}{project}");
            DotNetCorePublish ($"./src/{project}/", publishSettings);
        }
    });

//容器化
Task("Dockerize")
    .Does(() => {
        // var loginSettings = new  DockerRegistryLoginSettings { Username = "yongegames" };
        // DockerLogin(loginSettings, harborUrl);
        var stamp = TimeStamp();
        foreach (var project in projects) {
            Information ($"Dockerize {project}!");
            var projPath = $"{outputPath}{project}";
            var imageName = project.Split('.')[1].ToLower();
            var stampTag =  $"{registry}{imageName}:{stamp}";
            var latestTag =  $"{registry}{imageName}:{latest}";
            var buildSettings = new DockerImageBuildSettings { 
                File = $"{projPath}/Dockerfile.remote",
                Tag = new string [] { stampTag , latestTag },
            };
            //DeleteTag(imageName, latestTag).GetAwaiter().GetResult();
            DockerBuild(buildSettings, projPath);
            var pushSetting = new DockerImagePushSettings { DisableContentTrust = true    };
            //DockerPush(pushSetting, stampTag);           
            DockerPush(pushSetting, latestTag);           
        }
    });  

//构建并推送镜像
Task("Push")
    .IsDependentOn("Clean")
    .IsDependentOn("Publish")
    .IsDependentOn("Dockerize");

//构建镜像并且发布到服务器
Task("Deploy")
    .Does(()=> {
        var triggerUrl = $"{jenkinsUrl}/job/cluster/build?token=" + jenkinsToken;
        Information(HttpGet(triggerUrl));
    });

Task ("Default")
    .IsDependentOn ("Clean")
    .IsDependentOn ("Publish")
    .Does (() => {
        Information ($"Build Completed!");
    });

RunTarget(target);
