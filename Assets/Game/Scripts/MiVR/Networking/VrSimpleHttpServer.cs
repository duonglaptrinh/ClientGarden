using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Game.Client;
using Game.Client.Extension;
using TWT.Model;
using TWT.Networking.Config;
using UnityEngine;
using Game.Log;

namespace TWT.Networking.Server
{
    public class VrSimpleHttpServer
    {
        private readonly string[] indexFiles =
        {
            "index.html",
            "index.htm",
            "default.html",
            "default.htm"
        };

        private static readonly IDictionary<string, string> MimeTypeMappings =
            new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
            {
                #region extension to MIME type list

                {".asf", "video/x-ms-asf"},
                {".asx", "video/x-ms-asf"},
                {".avi", "video/x-msvideo"},
                {".bin", "application/octet-stream"},
                {".cco", "application/x-cocoa"},
                {".crt", "application/x-x509-ca-cert"},
                {".css", "text/css"},
                {".deb", "application/octet-stream"},
                {".der", "application/x-x509-ca-cert"},
                {".dll", "application/octet-stream"},
                {".dmg", "application/octet-stream"},
                {".ear", "application/java-archive"},
                {".eot", "application/octet-stream"},
                {".exe", "application/octet-stream"},
                {".flv", "video/x-flv"},
                {".gif", "image/gif"},
                {".hqx", "application/mac-binhex40"},
                {".htc", "text/x-component"},
                {".htm", "text/html"},
                {".html", "text/html"},
                {".ico", "image/x-icon"},
                {".img", "application/octet-stream"},
                {".iso", "application/octet-stream"},
                {".jar", "application/java-archive"},
                {".jardiff", "application/x-java-archive-diff"},
                {".jng", "image/x-jng"},
                {".jnlp", "application/x-java-jnlp-file"},
                {".jpeg", "image/jpeg"},
                {".jpg", "image/jpeg"},
                {".js", "application/x-javascript"},
                {".mml", "text/mathml"},
                {".mng", "video/x-mng"},
                {".mp4", "video/x-mp4"},
                {".mov", "video/quicktime"},
                {".mp3", "audio/mpeg"},
                {".mpeg", "video/mpeg"},
                {".mpg", "video/mpeg"},
                {".msi", "application/octet-stream"},
                {".msm", "application/octet-stream"},
                {".msp", "application/octet-stream"},
                {".pdb", "application/x-pilot"},
                {".pdf", "application/pdf"},
                {".pem", "application/x-x509-ca-cert"},
                {".pl", "application/x-perl"},
                {".pm", "application/x-perl"},
                {".png", "image/png"},
                {".prc", "application/x-pilot"},
                {".ra", "audio/x-realaudio"},
                {".rar", "application/x-rar-compressed"},
                {".rpm", "application/x-redhat-package-manager"},
                {".rss", "text/xml"},
                {".run", "application/x-makeself"},
                {".sea", "application/x-sea"},
                {".shtml", "text/html"},
                {".sit", "application/x-stuffit"},
                {".swf", "application/x-shockwave-flash"},
                {".tcl", "application/x-tcl"},
                {".tk", "application/x-tcl"},
                {".txt", "text/plain"},
                {".war", "application/java-archive"},
                {".wbmp", "image/vnd.wap.wbmp"},
                {".wmv", "video/x-ms-wmv"},
                {".xml", "text/xml"},
                {".xpi", "application/x-xpinstall"},
                {".zip", "application/zip"},

                #endregion
            };

        private readonly string[] absolutePathApi =
        {
            HttpApiConfig.ABSOLUTE_PATH_CONTENTS,
            HttpApiConfig.ABSOLUTE_PATH_GET_CONTENT,
            HttpApiConfig.ABSOLUTE_PATH_ADJUST_CONTENT_JSON,
            HttpApiConfig.ABSOLUTE_PATH_ADJUST_CONTENT_TITLE,
            HttpApiConfig.ABSOLUTE_PATH_UPLOAD_IMAGE,
            HttpApiConfig.ABSOLUTE_PATH_GET_THETA_IMAGE
        };

        private Thread serverThread;
        private string rootDirectory;
        private HttpListener listener;
        private int port;

        public int Port
        {
            get { return port; }
            private set { }
        }

        /// <summary>
        /// Construct server with given port.
        /// </summary>
        /// <param name="path">Directory path to serve.</param>
        /// <param name="port">Port of the server.</param>
        public VrSimpleHttpServer(string path, int port)
        {
            this.Initialize(path, port);
        }

        /// <summary>
        /// Construct server with suitable port.
        /// </summary>
        /// <param name="path">Directory path to serve.</param>
        public VrSimpleHttpServer(string path)
        {
            //get an empty port
            TcpListener l = new TcpListener(IPAddress.Loopback, 0);
            l.Start();
            int port = ((IPEndPoint)l.LocalEndpoint).Port;
            l.Stop();
            this.Initialize(path, port);
        }

        private void Initialize(string path, int port)
        {
            this.rootDirectory = path;
            this.port = port;
            //Validate();
            serverThread = new Thread(() => Task.Factory.StartNew(Listen));
            serverThread.Start();
        }

        public void Validate()
        {
            if (Directory.Exists(rootDirectory))
            {
                var vr = VrResourceStruct.CreateInstance(rootDirectory, true);
                vr.AllSubFolderListMap.Keys.ForEach(s => DebugExtension.Log($"[Init HTTP server] Detect content: {s}"));
            }
            else
            {
                throw new FileNotFoundException(rootDirectory);
            }
        }

        /// <summary>
        /// Stop server and dispose all functions.
        /// </summary>
        public void Stop()
        {
            listener.Stop();
            serverThread.Abort();
        }

        private async Task Listen()
        {
            listener = new HttpListener();
            listener.Prefixes.Add("http://*:" + port.ToString() + "/");
            listener.Start();
            while (listener.IsListening)
            {
                try
                {
                    HttpListenerContext context = await listener.GetContextAsync();
                    Task.Run(() => Process(context));
                }
                catch (Exception ex)
                {
                    UnityLogCustom.DebugErrorLog($"[VrSimpleHttpServer] {ex}");
                }
            }
        }

        private void Process(HttpListenerContext context)
        {
            var absolutePath = context.Request.Url.LocalPath;
            var filename = absolutePath.Substring(1);
            UnityLogCustom.DebugLog($"[{context.Request.HttpMethod}] request from {context.Request.RemoteEndPoint} => {context.Request.Url}");
            if (string.IsNullOrEmpty(filename))
            {
                foreach (string indexFile in indexFiles)
                {
                    if (File.Exists(Path.Combine(rootDirectory, indexFile)))
                    {
                        filename = indexFile;
                        break;
                    }
                }
            }

            var path = Path.Combine(rootDirectory, filename);
            DebugExtension.LogError(path);
            if (File.Exists(path))
            {
                HandleSendFile(context, path);
            }
            else if (IsApi(absolutePath))
            {
                HandleApi(context);
            }
            else
            {
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            }

            context.Response.OutputStream.Close();
        }

        private void HandleSendFile(HttpListenerContext context, string path)
        {
            var buffer = new byte[1024 * 16];
            try
            {
                Stream input = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite,
                    buffer.Length, FileOptions.Asynchronous);

                context.Response.StatusCode = (int)HttpStatusCode.OK;
                //Adding permanent http response headers
                context.Response.ContentType = MimeTypeMappings.TryGetValue(Path.GetExtension(path), out var mime)
                    ? mime
                    : "application/octet-stream";
                context.Response.ContentLength64 = input.Length;
                context.Response.AddHeader("Date", DateTime.Now.ToString("r"));
                context.Response.AddHeader("Last-Modified", System.IO.File.GetLastWriteTime(path).ToString("r"));
                context.Response.AddHeader("Access-Control-Allow-Credentials", "true");
                context.Response.AddHeader("Access-Control-Allow-Headers", "Accept, X-Access-Token, X-Application-Name, X-Request-Sent-Time");
                context.Response.AddHeader("Access-Control-Allow-Methods", "GET, POST, OPTIONS");
                context.Response.AddHeader("Access-Control-Allow-Origin", "*");

                int nbytes;
                while ((nbytes = input.Read(buffer, 0, buffer.Length)) > 0)
                    context.Response.OutputStream.Write(buffer, 0, nbytes);
                input.Close();

                context.Response.OutputStream.Flush();
            }
            catch (Exception ex)
            {
                DebugExtension.LogError(ex);
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
        }

        private void HandleApi(HttpListenerContext context)
        {
            var httpMethod = context.Request.HttpMethod;

            if (httpMethod == HttpMethod.Get.Method)
            {
                if (context.Request.Url.AbsolutePath == HttpApiConfig.ABSOLUTE_PATH_CONTENTS) //            "/contents"
                {
                    try
                    {
                        var vrStruct = VrResourceStruct.CreateInstance(rootDirectory);
                        var allContents = vrStruct.AllSubFolderListMap.Keys
                            .Where(x => x != VrDomeAssetResourceNameDefine.SYSTEM_DATA_ROOT)
                            .Select(contentName =>
                            {
                                var title = JsonUtility.FromJson<VrContentTitle>(
                                    File.ReadAllText(GetVrContentTitlePath(contentName)));
                                return new ContentInfo()
                                {
                                    contentName = contentName,
                                    contentTitle = title
                                };
                            }).ToArray();
                        var responseString = JsonUtility.ToJson(new GetAllContentDataResponse()
                        {
                            allContents = allContents,
                        });
                        DebugExtension.Log(responseString);
                        SendJson(context, responseString);
                    }
                    catch (Exception e)
                    {
                        UnityLogCustom.DebugErrorLog($"[HandleApi][{HttpApiConfig.ABSOLUTE_PATH_CONTENTS}] {e}");
                        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    }
                }
                else
                {
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                }
            }
            else if (httpMethod == HttpMethod.Post.Method)
            {
                if (context.Request.Url.AbsolutePath == HttpApiConfig.ABSOLUTE_PATH_GET_CONTENT) //    "/getContent"
                {
                    try
                    {
                        var request = GetContentRequestFromRequest<GetContentAbsoluteRequest>(context.Request);
                        UnityLogCustom.DebugLog($"[{GameContext.GameMode}][{request.contentName}] Client get content");

                        var responseString = GetContentAbsoluteResponseJson(request.contentName, request.contentType);

                        SendJson(context, responseString);

                    }
                    catch (Exception e)
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        UnityLogCustom.DebugErrorLog($"[HandleApi][{HttpApiConfig.ABSOLUTE_PATH_GET_CONTENT}] {e}");
                    }
                }
                else if (context.Request.Url.AbsolutePath == HttpApiConfig.ABSOLUTE_PATH_ADJUST_CONTENT_JSON) //     "/adjustContentJson"
                {
                    try
                    {
                        var request = GetContentRequestFromRequest<UpdateVrContentRequest>(context.Request);

                        var jsonPath = GetVrJsonPath(request.contentName);
                        var originJsonPath = GetOriginVrJsonPath(request.contentName);
                        var oldVersionPath = GetOldVrJsonPath(request.contentName);

                        System.IO.File.Move(jsonPath, oldVersionPath);
                        System.IO.File.WriteAllText(jsonPath, JsonUtility.ToJson(request.contentData));
                        System.IO.File.WriteAllText(originJsonPath, JsonUtility.ToJson(request.contentData));
                        UnityLogCustom.DebugLog($"{LogTag.EDIT_MODE}[{request.contentName}] Edited content. Old data on {oldVersionPath}");

                        var responseString = JsonUtility.ToJson(new UpdateVrContentResponse()
                        {
                            contentData = request.contentData,
                        });

                        SendJson(context, responseString);
                    }
                    catch (Exception e)
                    {
                        UnityLogCustom.DebugErrorLog($"[HandleApi][{HttpApiConfig.ABSOLUTE_PATH_ADJUST_CONTENT_JSON}] {e}");
                        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    }
                }
                else if (context.Request.Url.AbsolutePath == HttpApiConfig.ABSOLUTE_PATH_ADJUST_CONTENT_TITLE) //      "/adjustContentTitle"
                {
                    try
                    {
                        var request = GetContentRequestFromRequest<UpdateVrContentTitleRequest>(context.Request);

                        var contentTitleJsonPath = GetVrContentTitlePath(request.contentName);
                        var originContentTitleJsonPath = GetOriginVrContentTitlePath(request.contentName);

                        System.IO.File.WriteAllText(contentTitleJsonPath, JsonUtility.ToJson(request.vrContentTitle));
                        System.IO.File.WriteAllText(originContentTitleJsonPath, JsonUtility.ToJson(request.vrContentTitle));

                        var responseString = JsonUtility.ToJson(new UpdateVrContentTitleResponse()
                        {
                            contentName = request.contentName,
                            vrContentTitle = request.vrContentTitle,
                        });

                        SendJson(context, responseString);
                    }
                    catch (Exception e)
                    {
                        UnityLogCustom.DebugErrorLog($"[HandleApi][{HttpApiConfig.ABSOLUTE_PATH_ADJUST_CONTENT_TITLE}] {e}");
                        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    }
                }
                else
                {
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                }
            }
        }

        private static void SendJson(HttpListenerContext context, string responseString)
        {
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);

            context.Response.AddHeader("Access-Control-Allow-Credentials", "true");
            context.Response.AddHeader("Access-Control-Allow-Headers", "Accept, X-Access-Token, X-Application-Name, X-Request-Sent-Time");
            context.Response.AddHeader("Access-Control-Allow-Methods", "GET, POST, OPTIONS");
            context.Response.AddHeader("Access-Control-Allow-Origin", "*");

            context.Response.StatusCode = (int)HttpStatusCode.OK;
            context.Response.ContentLength64 = buffer.Length;
            context.Response.OutputStream.Write(buffer, 0, buffer.Length);
            context.Response.OutputStream.Flush();
        }

        private bool IsApi(string absolutePath)
        {
            return absolutePathApi.Any(s => s == absolutePath);
        }

        private static T GetContentRequestFromRequest<T>(HttpListenerRequest request)
        {
            System.IO.Stream body = request.InputStream;
            System.Text.Encoding encoding = request.ContentEncoding;
            System.IO.StreamReader reader = new System.IO.StreamReader(body, encoding);
            var stringRequest = reader.ReadToEnd();
            return JsonUtility.FromJson<T>(stringRequest);
        }

        private string[] GetPath(string contentName, int contentType)
        {
            var folderName = GetFolderNameByContentType(contentType);

            var vrStruct = VrResourceStruct.CreateInstance(rootDirectory);
            var folder = vrStruct.AllSubFolderListMap[contentName]
                .FirstOrDefault(fol => fol.Name == folderName);

            if (folder == null) return Array.Empty<string>();

            if (folderName != VrDomeAssetResourceNameDefine.IMAGE && folderName != VrDomeAssetResourceNameDefine.VIDEO
                    && folderName != VrDomeAssetResourceNameDefine.DOCUMENT && folderName != VrDomeAssetResourceNameDefine.VR_IMAGE
                    && folderName != VrDomeAssetResourceNameDefine.VR_VIDEO && folderName != VrDomeAssetResourceNameDefine.VR_MODEL
                    && folderName != VrDomeAssetResourceNameDefine.VR_PDF)
                return folder.PathFiles;
            var thumbnailFolderName = VrDomeAssetResourceNameDefine.IMAGE_THUMBNAIL;

            switch ((VrContentType)contentType)
            {
                case VrContentType.Image360:
                    thumbnailFolderName = VrDomeAssetResourceNameDefine.IMAGE_THUMBNAIL;
                    break;
                case VrContentType.Document:
                    thumbnailFolderName = VrDomeAssetResourceNameDefine.DOCUMENT_THUMBNAIL;
                    break;
                case VrContentType.Document_Video:
                    thumbnailFolderName = VrDomeAssetResourceNameDefine.DOCUMENT_VIDEO_THUMBNAIL;
                    break;
                case VrContentType.Movie360:
                    thumbnailFolderName = VrDomeAssetResourceNameDefine.VIDEO_THUMBNAIL;
                    break;
                case VrContentType.VrImage:
                    thumbnailFolderName = VrDomeAssetResourceNameDefine.VR_IMAGE_THUMBNAIL;
                    break;
                case VrContentType.VrVideo:
                    thumbnailFolderName = VrDomeAssetResourceNameDefine.VR_VIDEO_PREVIEW;
                    break;
                case VrContentType.VrModel:
                    thumbnailFolderName = VrDomeAssetResourceNameDefine.VR_MODEL_THUMBNAIL;
                    break;
                case VrContentType.VrPdf:
                    thumbnailFolderName = VrDomeAssetResourceNameDefine.VR_PDF_THUMBNAIL;
                    break;
            }

            var pathsImage = folder.PathFiles;

            var folderThumbnail = folder.Folders
                .FirstOrDefault(x => x.Name == thumbnailFolderName);
            var pathsThumbnail = folderThumbnail != null ? folderThumbnail.PathFiles : Array.Empty<string>();

            var paths = new string[pathsImage.Length + pathsThumbnail.Length];
            pathsImage.CopyTo(paths, 0);
            pathsThumbnail.CopyTo(paths, pathsImage.Length);
            return paths;
        }

        public string GetContentAbsoluteResponseJson(string contentName, int contentType)
        {
            var absolutePaths = GetPath(contentName, contentType)
                .Select(path => path.Replace(rootDirectory, ""))
                .ToArray();

            var responseString = JsonUtility.ToJson(new GetContentAbsoluteResponse()
            {
                contentType = contentType,
                absolutePaths = absolutePaths
            });

            return responseString;
        }

        private string GetFolderNameByContentType(int contentTypeValue)
        {
            switch ((VrContentType)contentTypeValue)
            {
                case VrContentType.Image360:
                    return VrDomeAssetResourceNameDefine.IMAGE;
                case VrContentType.Movie360:
                    return VrDomeAssetResourceNameDefine.VIDEO;
                case VrContentType.Document:
                    return VrDomeAssetResourceNameDefine.DOCUMENT;
                case VrContentType.Document_Video:
                    return VrDomeAssetResourceNameDefine.DOCUMENT_VIDEO_THUMBNAIL;
                case VrContentType.VrMark:
                    return VrDomeAssetResourceNameDefine.VR_MARK;
                case VrContentType.VrArrow:
                    return VrDomeAssetResourceNameDefine.VR_MOVE_ARROW;
                case VrContentType.VrJson:
                    return VrDomeAssetResourceNameDefine.VR_JSON;
                case VrContentType.VrImage:
                    return VrDomeAssetResourceNameDefine.VR_IMAGE;
                case VrContentType.VrVideo:
                    return VrDomeAssetResourceNameDefine.VR_VIDEO;
                case VrContentType.VrSound:
                    return VrDomeAssetResourceNameDefine.VR_SOUND;
                case VrContentType.VrModel:
                    return VrDomeAssetResourceNameDefine.VR_MODEL;
                case VrContentType.VrPdf:
                    return VrDomeAssetResourceNameDefine.VR_PDF;
                default:
                    throw new ArgumentOutOfRangeException(nameof(contentTypeValue), contentTypeValue, null);
            }
        }

        private string GetVrJsonPath(string contentName)
        {
            return Path.Combine(GetVrJsonFolder(contentName), VrContentFileNameConstant.VR_DATA_JSON);
        }

        private string GetOriginVrJsonPath(string contentName)
        {
            return Path.Combine(GetOriginVrJsonFolder(contentName), VrContentFileNameConstant.VR_DATA_JSON);
        }

        private string GetOldVrJsonPath(string contentName)
        {
            var logFolder = Path.GetDirectoryName(UnityLogCustom.LogPath);
            var oldVersionContentDataFolder = Path.Combine(logFolder, "OldVersionContentData");
            if (!Directory.Exists(oldVersionContentDataFolder))
            {
                Directory.CreateDirectory(oldVersionContentDataFolder);
            }

            var contentFolder = Path.Combine(oldVersionContentDataFolder, contentName);
            if (!Directory.Exists(contentFolder))
            {
                Directory.CreateDirectory(contentFolder);
            }

            return Path.Combine(contentFolder, $"vr-data_{DateTime.Now.ToString("yyyyMMdd_HHmmss")}.json");
        }

        private string GetVrJsonFolder(string contentName)
        {
            var absolutePath =
                $"{VrDomeAssetResourceNameDefine.VR_DATA_ROOT}/{VrDomeAssetResourceNameDefine.CONTENT_DATA_ROOT}/{contentName}/{VrDomeAssetResourceNameDefine.VR_JSON}";
            var path = Path.Combine(rootDirectory, absolutePath);

            return path;
        }

        private string GetOriginVrJsonFolder(string contentName)
        {
            var absolutePath =
               $"{VrDomeAssetResourceNameDefine.VR_DATA_ROOT}/{VrDomeAssetResourceNameDefine.CONTENT_DATA_ROOT}/{contentName}/{VrDomeAssetResourceNameDefine.VR_JSON}";

            var path = Path.Combine(GameContext.VrResourceRootPathOnServer, absolutePath);

            return path;
        }

        private string GetVrContentFolderPath(string contentName)
        {
            var absolutePath =
                $"{VrDomeAssetResourceNameDefine.VR_DATA_ROOT}/{VrDomeAssetResourceNameDefine.CONTENT_DATA_ROOT}/{contentName}";

            var path = Path.Combine(rootDirectory, absolutePath);

            return path;
        }

        private string GetOriginVrContentFolderPath(string contentName)
        {
            var absolutePath =
                $"{VrDomeAssetResourceNameDefine.VR_DATA_ROOT}/{VrDomeAssetResourceNameDefine.CONTENT_DATA_ROOT}/{contentName}";

            var path = Path.Combine(GameContext.VrResourceRootPathOnServer, absolutePath);

            return path;
        }

        private string GetVrContentTitlePath(string contentName)
        {
            return Path.Combine(GetVrContentFolderPath(contentName), VrContentFileNameConstant.CONTENT_DATA_TITLE_JSON);
        }

        private string GetOriginVrContentTitlePath(string contentName)
        {
            return Path.Combine(GetOriginVrContentFolderPath(contentName), VrContentFileNameConstant.CONTENT_DATA_TITLE_JSON);
        }

        private string GetOldVrContentTitlePath(string contentName)
        {
            return Path.Combine(GetVrContentFolderPath(contentName), $"title_{DateTime.Now.ToString("yyyyMMdd_HHmmss")}.json");
        }

        private string GetVrContentIconPath(string contentName)
        {
            return Path.Combine(GetVrContentFolderPath(contentName), VrContentFileNameConstant.CONTENT_DATA_ICON);
        }
    }
}