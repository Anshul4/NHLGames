﻿Imports System.IO
Imports System.Security.Cryptography
Imports NHLGames.My.Resources
Imports NHLGames.Utilities

Namespace Objects

    Public Class GameWatchArguments

        Public Property Quality As StreamQuality = StreamQuality.best
        Public Property Is60Fps As Boolean = True
        Public Property Cdn As CdnType = CdnType.Akc
        Public Property Stream As GameStream = Nothing
        Public Property GameTitle As String = String.Empty
        Public Property PlayerPath As String = String.Empty
        Public Property PlayerType As PlayerTypeEnum = PlayerTypeEnum.None
        Public Property StreamerPath As String = String.Empty
        Public Property UseStreamerArgs As Boolean = False
        Public Property StreamerArgs As String = String.Empty
        Public Property UsePlayerArgs As Boolean = False
        Public Property PlayerArgs As String = String.Empty
        Public Property UseOutputArgs As Boolean = False
        Public Property PlayerOutputPath As String = String.Empty
        Public Property StreamerType As StreamerTypeEnum = StreamerTypeEnum.None

        Public Overrides Function ToString() As String
            Return OutputArgs(False)
        End Function

        Public Overloads Function ToString(ByVal safeOutput As Boolean)
            Return OutputArgs(safeOutput)
        End Function

        Private Function GetStreamQuality() As String
            Dim selectedQualities = ""
            Dim addQuality = Quality
            Dim maxQuality = CType([Enum].GetValues(GetType(StreamQuality)), Integer()).Max()
            While addQuality < maxQuality
                selectedQualities &= addQuality.ToString().Substring(1) & ","
                addQuality = addQuality + 1
            End While
            selectedQualities &= StreamQuality.worst.ToString()
            Return selectedQualities
        End Function

        Private Function OutputArgs(ByVal safeOutput As Boolean)
            Dim returnValue As String = ""
            Const dblQuot As String = """"
            Const dblQuot2 As String = """"""
            Const space As String = " "
            Dim literalPlayerArgs As String = String.Empty

            Dim streamQuality = GetStreamQuality()

            If UsePlayerArgs Then
                literalPlayerArgs = PlayerArgs
            End If

            'DEBUG
            'Returnvalue &= "-v -l debug -h "

            Dim titleArg As String = literalPlayerArgs

            If PlayerType = PlayerTypeEnum.Vlc Then
                titleArg = String.Format("--meta-title{0}{1}{2}{1}{0}", space, dblQuot2, GameTitle)
            ElseIf PlayerType = PlayerTypeEnum.Mpv Then
                titleArg = String.Format("--force-window=immediate{0}--title{0}{1}{2}{1}{0}--user-agent=User-Agent={1}{3}{1}{0}", space, dblQuot2, GameTitle, Common.UserAgent)
            End If

            If String.IsNullOrEmpty(PlayerPath) = False Then
                returnValue &= String.Format("--player{0}{1}{2}{0}{3}{4}{0}{1}{0}", space, dblQuot, PlayerPath, titleArg, literalPlayerArgs)
            Else
                Console.WriteLine(English.errorPlayerPathEmpty)
            End If

            If PlayerType = PlayerTypeEnum.Mpv Then
                returnValue &= String.Format("--player-passthrough=hls{0}", space)
            End If

            If safeOutput = False Then
                returnValue &= String.Format("--http-cookie={0}mediaAuth={1}{2}{0}{2}", dblQuot, Common.GetRandomString(240), space)
            End If

            returnValue &= String.Format("--http-header={0}User-Agent={2}{0}{1}", dblQuot, space, Common.UserAgent)

            If safeOutput = False Then
                returnValue &= String.Format("{0}hlsvariant://", dblQuot)

                If Stream.IsVod Then
                    returnValue &= Stream.VODURL
                Else
                    returnValue &= Stream.GameURL
                End If

                returnValue = returnValue.Replace("CDN", Cdn.ToString().ToLower()) & space
            Else
                returnValue &= String.Format("{0}hlsvariant://{1}{2}", dblQuot, English.msgCensoredStream, space)
            End If

            If Is60Fps Then
                returnValue &= String.Format("name_key=bitrate{0}{1}best{1}", dblQuot, space)
            Else
                returnValue &= dblQuot & space & streamQuality & space
            End If

            returnValue &= String.Format("--http-no-ssl-verify{0}", space)

            If UseOutputArgs Then
                Dim outputPath As String = PlayerOutputPath.
                        Replace("(DATE)", DateHelper.GetPacificTime(Stream.Game.GameDate).ToString("yyyy-MM-dd")).
                        Replace("(HOME)", Stream.Game.HomeAbbrev).
                        Replace("(AWAY)", Stream.Game.AwayAbbrev).
                        Replace("(TYPE)", Stream.Type.ToString()).
                        Replace("(NETWORK)", Stream.Network)
                Dim suffix As Integer = 1
                Dim originalName = Path.GetFileNameWithoutExtension(outputPath)
                Dim originalExt = Path.GetExtension(outputPath)
                While (My.Computer.FileSystem.FileExists(outputPath))
                    outputPath = Path.ChangeExtension(Path.Combine(Path.GetDirectoryName(outputPath), originalName & "_" & suffix), originalExt)
                    suffix += 1
                End While

                returnValue &= "-f -o" & space & dblQuot & outputPath & dblQuot & space
            End If

            If UseStreamerArgs Then
                returnValue &= StreamerArgs
            End If

            Return returnValue
        End Function

End Class
End Namespace
